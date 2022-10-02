using LeDi.Display.Effects;
using Newtonsoft.Json;
using rpi_ws281x;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;

namespace LeDi.Display.Display
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

        public static void Calibrate()
        {
            if (LedCount == 0)
            {
                Console.WriteLine("Please set X and Y first");
                return;
            }

            Console.WriteLine("Starting all pixel change benchmark...");

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

            Console.WriteLine("full pixel change FPS: {0} FPS", FPS);
        }

        public static void Initialize(Layout layout)
        {
            LoadLayout(layout);
            LoadCharacters();

            if (LayoutConfig == null)
            {
                Console.WriteLine("LayoutConfig not initialized.");
                return;
            }

            Console.WriteLine("Initializing Controller for {0} with {1} LEDs on PWM0, DMA Channel {2} and a Frequency of {3}Hz", layout.Name, LedCount, LayoutConfig.DmaChannel, LayoutConfig.Frequency);

            //ControllerSettings = Settings.CreateDefaultSettings(); //800kHz and DMA Channel 10
            ControllerSettings = new Settings(LayoutConfig.Frequency, LayoutConfig.DmaChannel); //Using DMA Channel 10 limits the number of LEDs to 400 on a Raspberry Pi 3b. Don't know why
            Controller = ControllerSettings.AddController(LedCount, StripType.WS2812_STRIP, ControllerType.PWM0, Brightness, false);
            WS281X = new WS281x(ControllerSettings);

            SetAll(Color.Black);

            ChangeQueue = new Queue<Tuple<string, string, DateTime?>>();
            _TmrWorker.Elapsed += TmrWorker_Elapsed;
            _TmrWorker.Start();
        }

        public static void LoadLayout(Layout layoutSetting)
        {
            Console.WriteLine("Loading layout {0}", layoutSetting.Name);
            var layoutFilePath = string.Format("Config/Layouts/{0}.layout", layoutSetting.Name);

            if (!File.Exists(layoutFilePath))
            {
                Console.WriteLine("Layout file not exists. Please create file {0}", layoutFilePath);
                return;
            }

            StreamReader sR = new(layoutFilePath, Encoding.Default);
            var layout = sR.ReadToEnd();
            sR.Close();

            LayoutConfig = JsonConvert.DeserializeObject<Layout>(layout);

            if (LayoutConfig != null)
            {
                Console.WriteLine("Layout loaded successfully default settings");

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
                Console.WriteLine("Loading default layout FAILED");
            }
        }

        public static void LoadCharacters()
        {
            if (LayoutConfig == null)
            {
                Console.WriteLine("Layoutconfig not loaded.");
                return;
            }

            Console.WriteLine("Loading characters {0}", LayoutConfig.CharacterSet);
            var characterPath = string.Format("Config/Characters/{0}", LayoutConfig.CharacterSet);
            if (!Directory.Exists(characterPath))
            {
                Console.WriteLine("CharacterSet not found");
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
                            Console.WriteLine("Failed loading Character {0} in line {1} and resolution {2}", charDef.Char, lineCount, aResolutionFolder);
                            Console.WriteLine(ea.ToString());
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
                Console.Write("Controller is null. Not setting all LEDs.");
                return;
            }

            //color = Color.FromArgb(color.R * Brightness / 255, color.G * Brightness / 255, color.B * Brightness / 255);

            Controller.SetLED(LedNumber, color);
        }
        public static void SetAll(Color color)
        {
            if (Controller == null)
            {
                Console.Write("Controller is null. Not setting all LEDs.");
                return;
            }
            //color = Color.FromArgb(color.R * Brightness / 255, color.G * Brightness / 255, color.B * Brightness / 255);

            // Does not work for some reason
            //Controller.SetAll(color);

            for (var i = 0; i < Controller.LEDCount; i++)
            {
                SetLed(i, color);
            }            
        }

        public static void SetBrightness(byte brightness)
        {
            if (Controller == null || WS281X == null)
            {
                Console.Write("Controller is null. Not setting brightness.");
                return;
            }
            Console.WriteLine("brightness: "+ WS281X.GetBrightness());
            WS281X.SetBrightness(brightness);
        }

        public static void Render()
        {
            if (WS281X == null)
                return;

            var start = DateTime.Now;
            Console.Write("Rendering...");
            WS281X.Render();
            Console.WriteLine(" Done ({0}ms)", DateTime.Now.Subtract(start).TotalMilliseconds);
        }

        public static void ShowString(string text, string? areaName = null, string? characterSet = null, bool finalRender = false, int maxHeight = int.MaxValue, int maxWidth = int.MaxValue)
        {
            if (LayoutConfig == null || CharacterSets == null)
            {
                Console.WriteLine("LayoutConfig or CharactersSets is null");
                return;
            }

            if (LayoutConfig.AreaList == null)
                return;

            var area = areaName == null ? new Area() { Width = X, Height = Y } : LayoutConfig.AreaList.Single(x => x.Name == areaName);

            var matchingCharSets = CharacterSets.Where(x => x.Name == (characterSet ?? CharacterSet));
            matchingCharSets = matchingCharSets.Where(x => x.Height <= area.Height && x.Height <= maxHeight && x.Width <= maxWidth).OrderByDescending(x => x.Height).ThenByDescending(x => x.Width);
            var charSet = matchingCharSets.First();

            Console.WriteLine("Writing string {0} in area {1} using charSet {2}", text, area.Name, charSet.Name + "(" + charSet.Width + "x" + charSet.Height + ")");

            //Flush Area
            for (int x = area.PositionX; x < area.PositionX + area.Width; x++)
            {
                for (int y = area.PositionY; y < area.PositionY + area.Height; y++)
                {
                    Display.SetLed(Display.GetLedNumber(x, y), Color.Black);
                }
            }

            //Set the new text
            if (area.Align == "left")
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
                        Console.WriteLine("Skipping character {0} because it was not found.", aChar);
                        continue;
                    }

                    for (int x = 0; x < charObj.Width; x++)
                    {
                        for (int y = 0; y < charObj.Height; y++)
                        {
                            //If the Area Borders are hard and content should be cut off, don't show pixels out of area.
                            if (LayoutConfig.HardAreaBorders && (posX + x > area.Width + area.PositionX || posY + y > area.Height + area.PositionY || posX + x < area.PositionX || posY + y < area.PositionY))
                                continue;

                            var ledNum = GetLedNumber(posX + x, posY + y);
                            //Console.WriteLine("Setting X={0}/{1} and Y={2}/{3} with LED Number {4}", area.PositionX, x, area.PositionY, y, ledNum);

                            var brightnessfactor = charObj.Pixels[x, y].A / 255;
                            var color = Color.FromArgb(r * brightnessfactor, g * brightnessfactor, b * brightnessfactor);
                            SetLed(ledNum, color);
                        }
                    }

                    posX += charObj.Width + 1;
                }
            }
            else if (area.Align == "center")
            {
                //Get the width of the string
                var textWidth = 0;
                foreach (var aChar in text.ToCharArray())
                {
                    if (charSet == null || charSet.Characters == null)
                    {
                        Console.WriteLine("charSet is not loaded.");
                        continue;
                    }

                    var theChar = charSet.Characters.SingleOrDefault(x => x.Char == aChar);

                    if (theChar == null)
                    {
                        Console.WriteLine("Cannot find character {0}", aChar);
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
                        Console.WriteLine("Skipping character {0} because it was not found.", aChar);
                        continue;
                    }

                    for (int x = 0; x < charObj.Width; x++)
                    {
                        for (int y = 0; y < charObj.Height; y++)
                        {
                            //If the Area Borders are hard and content should be cut off, don't show pixels out of area.
                            if (LayoutConfig.HardAreaBorders && (posX + centerOffsetX + x > area.Width + area.PositionX || posY + y > area.Height + area.PositionY || posX + centerOffsetX + x < area.PositionX || posY + y < area.PositionY))
                                continue;

                            var ledNum = GetLedNumber(posX + x + centerOffsetX, posY + y);
                            //Console.WriteLine("Setting X={0}/{1} and Y={2}/{3} with LED Number {4}", area.PositionX, x, area.PositionY, y, ledNum);

                            var brightnessfactor = charObj.Pixels[x, y].A / 255;
                            var color = Color.FromArgb(r * brightnessfactor, g * brightnessfactor, b * brightnessfactor);
                            SetLed(ledNum, color);
                        }
                    }

                    posX += charObj.Width + 1;
                }
            }
            else if (area.Align == "right")
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
                        Console.WriteLine("Skipping character {0} because it was not found.", aChar);
                        continue;
                    }

                    //for (int x = charObj.Width-1; x >= 0; x--)
                    for (int x = 0; x < charObj.Width; x++)
                    {
                        for (int y = 0; y < charObj.Height; y++)
                        {
                            //Console.WriteLine("    Setting X={0}/{1} and Y={2}/{3}", posX, x, posY, y);

                            //If the Area Borders are hard and content should be cut off, don't show pixels out of area.
                            if (LayoutConfig.HardAreaBorders && (posX - x > area.Width + area.PositionX || posY + y > area.Height + area.PositionY || posX + x < area.PositionX || posY + y < area.PositionY))
                                continue;

                            var ledNum = GetLedNumber(posX - x, posY + y);
                            //Console.WriteLine("Setting X={0}/{1} and Y={2}/{3} with LED Number {4}", area.PositionX, x, area.PositionY, y, ledNum);
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
                Console.WriteLine("Layout Config not loaded.");
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
            Console.WriteLine("showing character {0} in area {1}", character, areaName);

            if (CharacterSets == null)
            {
                Console.WriteLine("CharacterSet not loaded.");
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
                Console.WriteLine("Cannot show character {0}", character);
                return;
            }

            ShowChar(charObj, areaName);
        }

        public static int GetLedNumber(int matrixX, int matrixY)
        {
            //Console.WriteLine("Get LED Number of X={0} Y={1}", matrixX, matrixY);

            //contains the modified X-Coordinated in case of alternating Rows
            var calculatedX = matrixX;
            var calculatedY = matrixY;

            //If the panel has alternating rows and it is an even row, inverse the LED count.
            if (HasAlternatingRows && matrixY % 2 != 0)
            {
                calculatedX = X - 1 - matrixX;
                //Console.WriteLine("Alternating Row. X is now {0}", calculatedX);
            }

            //If the LED #1 is at the bottom, switch Y axis
            if (IsBottomToTop)
            {
                calculatedY = Y - 1 - matrixY;
                // calculatedX = X - 1 - calculatedX;
                //Console.WriteLine("Switched Y={0} to Y={1}", matrixY, calculatedY);
            }

            //Switch the order from left to right in case the first LED is on the right
            if (!IsLeftToRight)
            {
                calculatedX = X - 1 - calculatedX;
            }

            //Console.WriteLine("LED Number is {0}", calculatedX + X * matrixY);
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

                //In case the change is expired, don't handle it and cann the Worker again
                if (change.Item3 != null && change.Item3 < DateTime.Now)
                {
                    Console.WriteLine("Text \"{0}\" expired.", change.Item2);
                    TmrWorker_Elapsed(sender, e);
                    return;
                }

                //Console.WriteLine("Writing Text \"{0}\" in area {1}", change.Item2, change.Item1.ToLower());

                //Set the string at the backend Matrix, but don't imediatly render and set the display
                ShowString(change.Item2, change.Item1.ToLower(), null, false);
            }

            //Render the changes
            Render();
        }
    }
}
