using LeDi.Display2.Effects;
using Newtonsoft.Json;
using rpi_ws281x;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using LeDi.Shared2.Display;

namespace LeDi.Display2.Display
{
    public static class Display
    {
        /// <summary>
        /// Returns the width of the LED Panel based on Layout Configuration
        /// </summary>
        public static int X { get { return LayoutConfig == null ? 0 : LayoutConfig.Width; } }

        /// <summary>
        /// Returns the height of the LED Panel based on Layout Configuration
        /// </summary>
        public static int Y { get { return LayoutConfig == null ? 0 : LayoutConfig.Height; } }

        /// <summary>
        /// Returns the brightness of the LED Panel based on Layout Configuration
        /// </summary>
        public static byte Brightness { get { return LayoutConfig == null ? (byte)0 : LayoutConfig.Brightness; } }

        /// <summary>
        /// Is the first line data line feed on the left and the second line on the right and so on?
        /// </summary>
        public static bool HasAlternatingRows { get; set; } = true;

        /// <summary>
        /// Is the data feed line at the bottom of the panel?
        /// </summary>
        public static bool IsBottomToTop { get; set; } = true;

        /// <summary>
        /// Is the data feed line at the left of the panel?
        /// </summary>
        public static bool IsLeftToRight { get; set; } = true;

        /// <summary>
        /// Returns the amount of LEDs of the panel
        /// </summary>
        public static int LedCount { get { return X * Y; } }

        /// <summary>
        /// Contains the Frames per Second after running Calibrate()
        /// </summary>
        public static double FPS { get => fPS; private set => fPS = value; }

        /// <summary>
        /// The CharacterSet name to use
        /// </summary>
        public static string CharacterSet { get => characterSetName; set => characterSetName = value; }

        /// <summary>
        /// The Layout loaded from config file
        /// </summary>
        public static Layout? LayoutConfig { get; set; }

        /// <summary>
        /// The configuration for the device
        /// </summary>
        public static DeviceConfig? Config { get; set; }

        /// <summary>
        /// All available Characters
        /// </summary>
        public static List<CharacterSet>? CharacterSets { get; set; }

        /// <summary>
        /// The Hardware Instance of the WS281X Library
        /// </summary>
        public static WS281x? WS281X { get; set; }

        /// <summary>
        /// The Settings for the WS281X Controller
        /// </summary>
        private static Settings? ControllerSettings { get; set; }

        /// <summary>
        /// The Controller for the WS281X Library
        /// </summary>
        private static Controller? Controller { get; set; }

        /// <summary>
        /// The changes to perform at the display received by application
        /// </summary>
        public static Queue<Tuple<string, string, DateTime?>>? ChangeQueue { get; set; }

        /// <summary>
        /// The timer, that will update the display every x ms. Default: 150ms
        /// </summary>
        private static readonly System.Timers.Timer _TmrWorker = new(150);
        private static double fPS;
        private static string characterSetName = "Default";
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly string ConfigFilename = "./config.json";

        public static void Calibrate()
        {
            if (LedCount == 0)
            {
                Logger.Error("Please set X and Y first");
                return;
            }

            Logger.Info("Starting all pixel change benchmark...");

            var start = DateTime.Now;

            //Performing 256 changes
            for (int i = 0; i < 256; i++)
            {
                switch (i % 3)
                {
                    case 0:
                        SetAll(Color.Red);
                        break;
                    case 1:
                        SetAll(Color.Green);
                        break;
                    case 2:
                        SetAll(Color.Blue);
                        break;
                }
                Render();
            }
            
            var end = DateTime.Now;

            // Change all back to black
            SetAll(Color.Black);
            Render();

            var diff2 = end.Subtract(start);
            FPS = 256 / diff2.TotalSeconds;

            Logger.Info("full pixel change FPS: {0} FPS", FPS);
        }

        public static async void Initialize(Layout layout)
        {
            LoadLayout(layout);
            LoadCharacters();

            if (LayoutConfig == null)
            {
                Logger.Warn("LayoutConfig not initialized.");
                return;
            }

            // Load Config
            Logger.Trace("Loading local device config...");

            if (!File.Exists(ConfigFilename))
            {
                Logger.Fatal("Unable to load config " + ConfigFilename);
                return;
            }

            Config = await LoadConfig();

            if (Config == null)
            {
                Logger.Fatal("Failed to load config.");
                return;
            }

            Logger.Info("Config {0} loaded.", ConfigFilename);            

            Logger.Info("Initializing Controller for {0} with {1} LEDs on PWM{4} on GPIO{5}, DMA Channel {2} and a Frequency of {3}Hz", layout.Name, LedCount, LayoutConfig.DmaChannel, LayoutConfig.Frequency, Config.PwmChannel, Config.GpioPin);

            //ControllerSettings = Settings.CreateDefaultSettings(); //800kHz and DMA Channel 10 is default
            ControllerSettings = new Settings(LayoutConfig.Frequency, LayoutConfig.DmaChannel); //Using DMA Channel 10 limits the number of LEDs to 400 on a Raspberry Pi 3b. Don't know why. DMA Channel 5 works with more

            // Set the pin object
            var pin = Pin.Gpio12;
            switch(Config.GpioPin)
            {
                case 18:
                    pin = Pin.Gpio18;
                    break;
                case 12:
                    pin = Pin.Gpio12;
                    break;
                case 13:
                    pin = Pin.Gpio13;
                    break;
                case 19:
                    pin = Pin.Gpio19;
                    break;
            }

            // Set the PWM object; Raspberry Pi has 2 channels. It can be 0 or 1.
            var pwmChannel = ControllerType.PWM0;
            if (Config.PwmChannel == 1)
                pwmChannel = ControllerType.PWM1;

            

            Controller = ControllerSettings.AddController(LedCount, pin, StripType.WS2812_STRIP, pwmChannel, Brightness, false);

            try
            {
                WS281X = new WS281x(ControllerSettings);
            }
            catch(Exception ex)
            {
#if DEBUG
                Logger.Error(ex, "Cannot load Raspberry Pi WS2812B Library. Are you running this on Raspberry or did you forgot to compile the ws2811.so?. Trying to continue without connection to display. This might result in crashes or unexpected behaviours.");
#endif
#if RELEASE
                throw new Exception("Cannot load Raspberry Pi WS2812B Library. Are you running this on Raspberry Pi (2 or newer, not Zero) or did you forgot to compile the ws2811.so?.");
#endif
            }


            SetAll(Color.Black);

            ChangeQueue = new Queue<Tuple<string, string, DateTime?>>();
            _TmrWorker.Elapsed += TmrWorker_Elapsed;
            _TmrWorker.Start();

            Controller.SetLED(0, Color.Green);
            Render();

            Logger.Debug("Initialization done.");
        }

        public static void LoadLayout(Layout layoutSetting)
        {
            Logger.Trace("Loading layout {0}", layoutSetting.Name);
            var layoutFilePath = string.Format("Config/Layouts/{0}.layout", layoutSetting.Name);

            if (!File.Exists(layoutFilePath))
            {
                Logger.Error("Layout file not exists. Please create file {0}", layoutFilePath);
                return;
            }

            StreamReader sR = new(layoutFilePath, Encoding.Default);
            var layout = sR.ReadToEnd();
            sR.Close();

            LayoutConfig = JsonConvert.DeserializeObject<Layout>(layout);

            if (LayoutConfig != null)
            {
                Logger.Debug("Layout loaded successfully default settings");

                //Setting the non-default settings received from server (manually set via UI)
                if (layoutSetting.Brightness > 0)
                    LayoutConfig.Brightness = layoutSetting.Brightness;

                if (layoutSetting.DmaChannel > 0)
                    LayoutConfig.DmaChannel = layoutSetting.DmaChannel;

                if (layoutSetting.CharacterSet != null)
                    LayoutConfig.CharacterSet = layoutSetting.CharacterSet;

                if (layoutSetting.Frequency > 0)
                    LayoutConfig.Frequency = layoutSetting.Frequency;

                LayoutConfig.HardAreaBorders = layoutSetting.HardAreaBorders;

                if (layoutSetting.Height > 0)
                    LayoutConfig.Height = layoutSetting.Height;

                if (layoutSetting.Width > 0)
                    LayoutConfig.Width = layoutSetting.Width;
            }
            else
            {
                Logger.Error("Loading default layout FAILED");
            }
        }

        public static void LoadCharacters()
        {
            if (LayoutConfig == null)
            {
                Logger.Error("Layoutconfig not loaded.");
                return;
            }

            Logger.Debug("Loading characters {0}", LayoutConfig.CharacterSet);
            var characterPath = string.Format("Config/Characters/{0}", LayoutConfig.CharacterSet);
            if (!Directory.Exists(characterPath))
            {
                Logger.Error("CharacterSet not found");
                return;
            }

            CharacterSets = new List<CharacterSet>();

            //Get each resolution:
            foreach (var aResolutionFolder in Directory.GetDirectories(characterPath))
            {
                var charSet = new CharacterSet();
                var directoryName = aResolutionFolder.Substring(aResolutionFolder.Replace('\\', '/').LastIndexOf('/') + 1);
                charSet.Width = Convert.ToInt32(directoryName.Split('x')[0]);
                charSet.Height = Convert.ToInt32(directoryName.Split('x')[1]);
                charSet.Name = LayoutConfig.CharacterSet;
                charSet.Characters = new List<Character>();

                if (LayoutConfig.CharacterList == null)
                    continue;

                foreach (var charDef in LayoutConfig.CharacterList)
                {
                    var sR = new StreamReader(aResolutionFolder + "/" + charDef.File, Encoding.Default);
                    var charFile = sR.ReadToEnd().Replace("\r", "").Split('\n');
                    sR.Close();

                    var charDefinition = new Character(charFile[0].Length, charFile.Length)
                    {
                        Name = charDef.Name,
                        Char = charDef.Char,
                        File = charDef.File
                    };

                    var lineCount = 0;
                    foreach (var charContentLine in charFile)
                    {
                        try
                        {
                            if (lineCount > charSet.Height) //Ignore lines, that will exceed the charater size
                                continue;

                            var charContent = charContentLine.ToCharArray();

                            //Get pixels, maximum the number of the width
                            for (var pxNum = 0; pxNum < charSet.Width && pxNum < charContent.Length; pxNum++)
                            {
                                if (charContent[pxNum] == ' ')
                                    continue;

                                var baseBrightness = (byte)(((int.Parse(charContent[pxNum].ToString(), System.Globalization.NumberStyles.HexNumber) + 1) * 16) - 1);
                                charDefinition.Pixels[pxNum, lineCount] = Color.FromArgb(baseBrightness, 0, 0, 0);
                            }
                        }
                        catch (Exception ea)
                        {
                            Logger.Error(ea, "Failed loading Character {0} in line {1} and resolution {2}", charDef.Char, lineCount, aResolutionFolder);
                        }
                        lineCount++;
                    }

                    //Add the character to the character Set
                    charSet.Characters.Add(charDefinition);
                }

                //add the character Set to the total list
                CharacterSets.Add(charSet);
            }
        }

        public static void SetLed(int LedNumber, Color color)
        {
            if (Controller == null)
            {
                Logger.Error("Controller is null. Not setting all LEDs.");
                return;
            }
            if (LedNumber >= Controller.LEDCount)
            {
                Logger.Error("LED {0} cannot being set because the display just has {1} LEDs.", LedNumber, Controller.LEDCount);
                return;
            }

            Controller.SetLED(LedNumber, color);
        }
        public static void SetAll(Color color)
        {
            if (Controller == null)
            {
                Logger.Error("Controller is null. Not setting all LEDs.");
                return;
            }

            // Does not work for some reason
            //Controller.SetAll(color);

            // Workaround for above. Actually it is doing the same as the SetAll function of the controller.
            for (var i = 0; i < Controller.LEDCount; i++)
            {
                SetLed(i, color);
            }            
        }

        public static void SetBrightness(byte brightness)
        {
            if (Controller == null || WS281X == null)
            {
                Logger.Error("Controller is null. Not setting brightness.");
                return;
            }
            Logger.Info("Brightness set to " + WS281X.GetBrightness());
            WS281X.SetBrightness(brightness);
        }

        /// <summary>
        /// Render the newly set changes
        /// </summary>
        public static void Render()
        {
            if (WS281X == null)
            {
                Logger.Warn("Cannot render. Strip not initialized.");
                return;
            }

            var start = DateTime.Now;
            Logger.Trace("Rendering...");
            WS281X.Render();
            Logger.Trace(" Done ({0}ms)", DateTime.Now.Subtract(start).TotalMilliseconds);
        }

        /// <summary>
        /// Shows a string on the display
        /// </summary>
        /// <param name="text"></param>
        /// <param name="areaName"></param>
        /// <param name="characterSet"></param>
        /// <param name="finalRender"></param>
        /// <param name="maxHeight"></param>
        /// <param name="maxWidth"></param>
        /// <param name="align"></param>
        public static void ShowString(string text, string? areaName = null, string? characterSet = null, bool finalRender = false, int maxHeight = int.MaxValue, int maxWidth = int.MaxValue, string align = "default")
        {
            if (LayoutConfig == null || CharacterSets == null)
            {
                Logger.Error("LayoutConfig or CharactersSets is null");
                return;
            }

            if (LayoutConfig.AreaList == null)
            {
                Logger.Error("No Arealist loaded.");
                return;
            }

            if (!LayoutConfig.AreaList.Select(x => x.Name).Contains(areaName) && areaName != null)
            {
                Logger.Warn("Area {0} not found to show string {1}.", areaName, text);
                return;
            }

            // Get the area details
            var area = areaName == null ? new Area() { Width = X, Height = Y } : LayoutConfig.AreaList.Single(x => x.Name == areaName);

            // Set the alignment in case an area was defined;
            if (areaName != null && align == "default")
            {
                align = area.Align;
            }

            // Get the CharacterSet Sizes
            var allCharSets = CharacterSets.Where(x => x.Name == (characterSet ?? CharacterSet));
            var matchingCharSets = allCharSets;

            // Automatically detect best size
            if (area.MaxCharSize == null)
            {
                Logger.Trace("Getting best Charsize by height and textlength.");
                // Get best charset that fits into the height and to total text length fits into the width of the area
                // Fits (best) to height
                var matchingCharSetsHeight = allCharSets
                    .Where(z => z.Height <= area.Height && z.Height <= maxHeight) 
                    .OrderByDescending(z => z.Height)
                    .ThenByDescending(z => z.Width);

                // Fits (best) to width
                var matchingCharSetsWidth = allCharSets
                    .Where(z => text.Length * (z.Width + 1) <= area.Width && (z.Width + 1) <= maxWidth) //z.Width+1 because there is one column of free pixels between the characters.
                    .OrderByDescending(z => z.Height)
                    .ThenByDescending(z => z.Width);

                // Get the charset name that fits both
                var matchingCharSetsBoth = matchingCharSetsHeight.Select(x => x.Width + "x" + x.Height).IntersectBy(matchingCharSetsWidth.Select(x => x.Width + "x" + x.Height), x => x);

                // Check if there is any matching charset. If not, take the smallest, that fits the height.
                if (matchingCharSetsBoth == null || matchingCharSetsBoth.Count() == 0)
                {
                    Logger.Trace("No charset fits length and height. Taking smallest height {0}", matchingCharSetsHeight.Last().Name);
                    matchingCharSets = new List<CharacterSet>() { matchingCharSetsHeight.Last() };
                }
                else // there is a least one matching charset
                {
                    Logger.Trace("Automatically detected charsets found: {0}", string.Join(", ", matchingCharSetsBoth));
                    matchingCharSets = matchingCharSets.Where(x => matchingCharSetsBoth.Contains(x.Width + "x" + x.Height));
                }
            }
            else // Get the best matching char size but respect the configured limit of the area
            {
                Logger.Trace("Get charset with maxcharsize (max: {0},{1})", area.MaxCharSize[0], area.MaxCharSize[1]);
                matchingCharSets = allCharSets.Where(x => x.Width <= area.MaxCharSize[0] && x.Height <= area.MaxCharSize[1]).OrderByDescending(x => x.Height).ThenByDescending(x => x.Width);
            }
            Logger.Trace("Found {0} matching charsets", matchingCharSets.Count());

            // Get best matching charset, but there will be problems fit them into the area or fulfill other requirements.
            if (!matchingCharSets.Any())
            {
                matchingCharSets = allCharSets.Where(x => x.Height <= area.Height).OrderByDescending(x => x.Height).ThenByDescending(x => x.Width);
                Logger.Warn("Using fallback charset, that does not fulfill all requirements...");
            }

            // Get the best matching set
            var charSet = matchingCharSets.First();

            Logger.Trace("Writing string {0} in area {1} using charSet {2}", text, area.Name, charSet.Name + "(" + charSet.Width + "x" + charSet.Height + ")");

            //Flush Area
            for (int x = area.PositionX; x < area.PositionX + area.Width; x++)
            {
                for (int y = area.PositionY; y < area.PositionY + area.Height; y++)
                {
                    SetLed(GetLedNumber(x, y), Color.Black);
                }
            }

            //Set the new text
            if (align == "left")
            {
                var posX = area.PositionX;
                var posY = area.PositionY;

                var r = int.Parse(area.Color.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                var g = int.Parse(area.Color.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                var b = int.Parse(area.Color.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);


                foreach (var aChar in text.ToCharArray())
                {
                    if (charSet.Characters == null)
                        continue;

                    var charObj = charSet.Characters.SingleOrDefault(x => x.Char == aChar);

                    if (charObj == null)
                    {
                        Logger.Warn("Skipping character {0} because it was not found.", aChar);
                        continue;
                    }

                    for (int x = 0; x < charObj.Width; x++)
                    {
                        for (int y = 0; y < charObj.Height; y++)
                        {
                            //If the Area Borders are hard and content should be cut off, don't show pixels out of area.
                            if (LayoutConfig.HardAreaBorders && (posX + x > area.Width + area.PositionX || posY + y > area.Height + area.PositionY || posX + x < area.PositionX || posY + y < area.PositionY))
                            {
                                Logger.Warn("Skipping pixel {0}/{1} for area {2} because it is out of area with hard borders {3}/{4}", posX + x, posY + y, area.Name, area.PositionX, area.PositionY);
                                continue;
                            }

                            //Check if coordinate is out of boundries
                            if (posX + x >= X || posY + y >= Y)
                            {
                                Logger.Warn("Skipping pixel {0}/{1} for area {2} because it is out of boundries {3}/{4}", posX + x, posY + y, area.Name, X, Y);
                                continue;
                            }

                            var ledNum = GetLedNumber(posX + x, posY + y);
                            //Console.WriteLine("Setting left X={0}/{1} and Y={2}/{3} with LED Number {4}", area.PositionX, x, area.PositionY, y, ledNum);

                            var brightnessfactor = charObj.Pixels[x, y].A / 255;
                            var color = Color.FromArgb(r * brightnessfactor, g * brightnessfactor, b * brightnessfactor);
                            SetLed(ledNum, color);
                        }
                    }

                    posX += charObj.Width + 1;
                }
            }
            else if (align == "center" || align == "default")
            {
                //Get the width of the string
                var textWidth = 0;
                foreach (var aChar in text.ToCharArray())
                {
                    if (charSet == null || charSet.Characters == null)
                    {
                        Logger.Error("charSet is not loaded.");
                        continue;
                    }

                    var theChar = charSet.Characters.SingleOrDefault(x => x.Char == aChar);

                    if (theChar == null)
                    {
                        Logger.Error("Cannot find character {0}", aChar);
                        continue;
                    }

                    textWidth += theChar.Width;
                    textWidth++;
                }
                textWidth--; //Substract the tailing character space

                //Move the current write-position to the centered position
                var posX = area.PositionX;
                var posY = area.PositionY;
                var centerOffsetX = area.Width / 2 - textWidth / 2;

                var r = int.Parse(area.Color.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                var g = int.Parse(area.Color.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                var b = int.Parse(area.Color.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

                //Console.WriteLine("posX={0},areaWidth={1},textWidth={2},centerOffsetX={3}", posX, area.Width, textWidth,centerOffsetX);

                //Set the new text centered
                foreach (var aChar in text.ToCharArray())
                {
                    if (charSet.Characters == null)
                        continue;

                    var charObj = charSet.Characters.SingleOrDefault(x => x.Char == aChar);

                    if (charObj == null)
                    {
                        Logger.Warn("Skipping character {0} because it was not found.", aChar);
                        continue;
                    }

                    for (int x = 0; x < charObj.Width; x++)
                    {
                        for (int y = 0; y < charObj.Height; y++)
                        {
                            //If the Area Borders are hard and content should be cut off, don't show pixels out of area.
                            if (LayoutConfig.HardAreaBorders && (posX + centerOffsetX + x > area.Width + area.PositionX || posY + y > area.Height + area.PositionY || posX + centerOffsetX + x < area.PositionX || posY + y < area.PositionY))
                            {
                                Logger.Warn("Skipping pixel {0}/{1} for area {2} because it is out of area with hard borders {3}/{4}", posX + centerOffsetX + x, posY + y, area.Name, area.PositionX, area.PositionY);
                                continue;
                            }

                            //Check if coordinate is out of boundries
                            if (posX + centerOffsetX + x >= X || posY + y >= Y)
                            {
                                Logger.Warn("Skipping pixel {0}/{1} for area {2} because it is out of boundries {3}/{4}", posX + centerOffsetX + x, posY + y, area.Name, X, Y);
                                continue;
                            }

                            var ledNum = GetLedNumber(posX + x + centerOffsetX, posY + y);
                            //Console.WriteLine("Setting center X={0}/{1} and Y={2}/{3} with LED Number {4}", area.PositionX, x, area.PositionY, y, ledNum);

                            var brightnessfactor = charObj.Pixels[x, y].A / 255;
                            var color = Color.FromArgb(r * brightnessfactor, g * brightnessfactor, b * brightnessfactor);
                            SetLed(ledNum, color);
                        }
                    }

                    posX += charObj.Width + 1;
                }
            }
            else if (align == "right")
            {
                var posX = area.PositionX + area.Width;
                var posY = area.PositionY;

                var r = int.Parse(area.Color.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                var g = int.Parse(area.Color.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                var b = int.Parse(area.Color.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                //Console.WriteLine("posX={0},areaWidth={1}", posX, area.Width);

                //Set the new text right. Take the last character first
                foreach (var aChar in text.ToCharArray().Reverse())
                {
                    if (charSet.Characters == null)
                        continue;

                    var charObj = charSet.Characters.SingleOrDefault(x => x.Char == aChar);
                    //Console.WriteLine("Printing Character {0}", aChar);

                    if (charObj == null)
                    {
                        Logger.Warn("Skipping character {0} because it was not found.", aChar);
                        continue;
                    }

                    for (int x = 0; x < charObj.Width; x++)
                    {
                        for (int y = 0; y < charObj.Height; y++)
                        {
                            //Console.WriteLine("    Setting X={0}/{1} and Y={2}/{3}", posX, x, posY, y);

                            //If the Area Borders are hard and content should be cut off, don't show pixels out of area.
                            if (LayoutConfig.HardAreaBorders && (posX - charObj.Width + x +1 > area.Width + area.PositionX || posY + y > area.Height + area.PositionY || posX - charObj.Width + x < area.PositionX || posY + y < area.PositionY))
                            {
                                Logger.Warn("Skipping pixel {0}/{1} for area {2} because it is out of area with hard borders {3}/{4}", posX - charObj.Width + x+1, posY + y, area.Name, area.PositionX, area.PositionY);
                                continue;
                            }

                            //Check if coordinate is out of boundries
                            if (posX - charObj.Width + x >= X || posY + y >= Y)
                            {
                                Logger.Warn("Skipping pixel {0}/{1} for area {2} because it is out of boundries {3}/{4}", posX - charObj.Width + x +1, posY + y, area.Name, X, Y);
                                continue;
                            }

                            var ledNum = GetLedNumber(posX - charObj.Width + x + 1, posY + y);
                            //Console.WriteLine("Setting right X={0}/{1} and Y={2}/{3} with LED Number {4}", area.PositionX, x, area.PositionY, y, ledNum);
                            var brightnessfactor = charObj.Pixels[x, y].A / 255;
                            var color = Color.FromArgb(r * brightnessfactor, g * brightnessfactor, b * brightnessfactor);
                            SetLed(ledNum, color);
                        }
                    }

                    posX -= charObj.Width + 1;
                }
            }
            if (finalRender) Render();
        }

        public static void ShowChar(Character charObj, string? areaName, bool finalRender = false)
        {
            if (LayoutConfig == null || areaName == null)
            {
                Logger.Error("Layout Config not loaded.");
                return;
            }

            if (LayoutConfig.AreaList == null)
                return;

            var area = LayoutConfig.AreaList.Single(x => x.Name == areaName);

            var r = int.Parse(area.Color.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            var g = int.Parse(area.Color.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            var b = int.Parse(area.Color.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

            for (int x = 0; x < charObj.Width; x++)
            {
                for (int y = 0; y < charObj.Height; y++)
                {
                    //If the Area Borders are hard and content should be cut off, don't show pixels out of area.
                    if (LayoutConfig.HardAreaBorders && (x + 1 > area.Width || y + 1 > area.Height))
                        continue;

                    var ledNum = GetLedNumber(area.PositionX + x, area.PositionY + y);
                    //Console.WriteLine("Setting X={0}/{1} and Y={2}/{3} with LED Number {4}", area.PositionX, x, area.PositionY, y, ledNum);

                    var brightnessfactor = charObj.Pixels[x, y].A / 255;
                    var color = Color.FromArgb(r * brightnessfactor, g * brightnessfactor, b * brightnessfactor);

                    SetLed(ledNum, color);
                }
            }
            if (finalRender) Render();
        }

        public static void ShowChar(char character, string areaName)
        {
            Logger.Debug("showing character {0} in area {1}", character, areaName);

            if (CharacterSets == null)
            {
                Logger.Error("CharacterSet not loaded.");
                return;
            }


            //Height = 10 for testing. Needs to be advanced to support multiple sizes
#pragma warning disable CS8604 // Possible null reference argument.
            var charObj = CharacterSets
                .Single(x => x.Name == CharacterSet && x.Height == 10).Characters
                .DefaultIfEmpty(new Character(6, 10) { Name = "Unknown" })
                .SingleOrDefault(x => x.Char == character);
#pragma warning restore CS8604 // Possible null reference argument.

            if (charObj == null)
            {
                Logger.Error("Cannot show character {0}", character);
                return;
            }

            ShowChar(charObj, areaName);
        }

        public static int GetLedNumber(int matrixX, int matrixY)
        {
            //contains the modified X-Coordinated in case of alternating Rows
            var calculatedX = matrixX;
            var calculatedY = matrixY;

            //If the panel has alternating rows and it is an even row, inverse the LED count.
            if (HasAlternatingRows && matrixY % 2 != 0)
            {
                calculatedX = X - 1 - matrixX;
                Logger.Trace("Alternating Row. X is now {0}", calculatedX);
            }

            //If the LED #1 is at the bottom, switch Y axis
            if (IsBottomToTop)
            {
                calculatedY = Y - 1 - matrixY;
                // calculatedX = X - 1 - calculatedX;
                Logger.Trace("Switched Y={0} to Y={1}", matrixY, calculatedY);
            }

            //Switch the order from left to right in case the first LED is on the right
            if (!IsLeftToRight)
            {
                calculatedX = X - 1 - calculatedX;
            }

            Logger.Trace("LED Number is {0}", calculatedX + X * matrixY);
            return calculatedX + X * calculatedY;
        }


        private static void TmrWorker_Elapsed(object? sender, ElapsedEventArgs e)
        {
            if (ChangeQueue == null || ChangeQueue.Count == 0)
                return;

            while (ChangeQueue.Count != 0)
            {
                var change = ChangeQueue.Dequeue();

                if (change == null)
                    return;

                //In case the change is expired, don't handle it and run the Worker again
                if (change.Item3 != null && change.Item3 < DateTime.Now)
                {
                    Logger.Debug("Text \"{0}\" expired.", change.Item2);
                    TmrWorker_Elapsed(sender, e);
                    return;
                }

                //Console.WriteLine("Writing Text \"{0}\" in area {1}", change.Item2, change.Item1.ToLower());

                //Set the string at the backend Matrix, but don't immediately render and set the display
                ShowString(change.Item2, change.Item1.ToLower(), null, false);
            }

            //Render the changes
            Render();
        }


        /// <summary>
        /// Loads the config from the config.json file
        /// </summary>
        private static async Task<DeviceConfig?> LoadConfig()
        {
            try
            {
                Logger.Info("Loading config {0}. Current Directory is {1}", ConfigFilename, Environment.CurrentDirectory);
                StreamReader sR = new StreamReader(ConfigFilename);
                var json = await sR.ReadToEndAsync();
                sR.Close();
                Logger.Trace("Loaded json content is {0}", json);
                var config = JsonConvert.DeserializeObject<DeviceConfig>(json, GetJsonSerializer());

                if (config != null)
                {
                    Logger.Debug("Server Setting value: {0}", config.ServerURL);
                    Logger.Info("Successfully loaded config.");
                    return config;
                }

                Logger.Error("Loading the config file returns a null value.");
                return null;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Unable to load config. Does file {0} exists?", ConfigFilename);
                return null;
            }
        }

        /// <summary>
        /// Helper method to get the Json Serializer settings
        /// </summary>
        /// <returns></returns>
        private static JsonSerializerSettings GetJsonSerializer()
        {
            var js = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                StringEscapeHandling = StringEscapeHandling.EscapeHtml
            };

            return js;
        }
    }
}
