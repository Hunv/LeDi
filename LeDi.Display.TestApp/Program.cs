using System;
using System.Drawing;
using System.Linq;
using LeDi.Display.Display;
using LeDi.Display.Effects;

namespace LedGameDisplayTestApp
{
    class Program
    {

        static void Main(string[] args)
        {
            // See https://aka.ms/new-console-template for more information
            Console.WriteLine("Hello, World!");


            var input = 0;
            do
            {
                Console.Clear();
                Console.WriteLine("What do you want to test:");
                Console.WriteLine("Press CTRL + C to abort to current test.");
                Console.WriteLine("0 - Exit");
                Console.WriteLine("1 - Color wipe animation");
                Console.WriteLine("2 - Rainbow color animation");
                Console.WriteLine("3 - fading left to right animation");
                Console.WriteLine("4 - Calibration Tasks");
                Console.WriteLine("5 - Chartest");
                Console.WriteLine("6 - Areatest");

                Console.Write("What is your choice: ");
                var answer = Console.ReadLine();
                if (answer == null)
                    continue;
                input = int.Parse(answer);

                var layout = new Layout();
                layout.Width = 20;
                layout.Height = 10;
                layout.Name = "20x10";
                Display.Initialize(layout);

                if (input != 4)
                {
                    var effect = GetEffect(input);
                    if (effect != null)
                    {
                        effect.Execute();
                    }
                }
                else if (input == 4)
                {
                    Display.Calibrate();
                }

            } while (input != 0);
        }


        private static IEffect? GetEffect(int input)
        {
            IEffect? result = null;

            switch (input)
            {
                case 1:
                    result = new TestPixelWipe();
                    Console.WriteLine("Select the Color (RGB Format)");
                    var hex = Console.ReadLine();

                    if (hex == null)
                        return null;

                    ((TestPixelWipe)result).Color = Color.FromArgb(Convert.ToByte(hex.Substring(0, 2), 16), Convert.ToByte(hex.Substring(2, 2), 16), Convert.ToByte(hex.Substring(4, 2), 16));
                    break;
                case 2:
                    Console.WriteLine("Not implemented");
                    Console.ReadLine();
                    break;
                case 3:
                    result = new IdleBar();
                    break;
                case 5:
                    result = new TestChar();
                    break;
                case 6:
                    result = new TestArea();
                    break;
            }

            return result;
        }
    }
}