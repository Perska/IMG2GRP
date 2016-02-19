using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace IMG2GRP
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("ERROR: Too many/Not enough arguments\nUsage: (executable name) <filename>\nWildcards are allowed.\nNOTICE: A GRP EXTENSION WILL BE ADDED TO THE END OF THE OUTPUT");
            }
            if (args.Length == 1)
            {
                string[] files = Directory.GetFiles(Directory.GetCurrentDirectory(), args[0], SearchOption.TopDirectoryOnly);
                foreach (string file in files)
                {
                    Console.WriteLine("------------\nConverting the image \"" + file + "\" to a grp...");
                    Bitmap tmp = new Bitmap(file);
                    Bitmap input = new Bitmap(512, 512);
                    Graphics a;
                    using (a = Graphics.FromImage(input))
                    {
                        a.DrawImageUnscaled(tmp, new Point(0, 0));
                    }
                    byte[] process = new byte[524416];
                    Console.WriteLine("Writing header & footer to file...");
                    hextoarray("01 00 01 00 00 00 02 00 1C 00 08 00 DF 07 0A 0F 00 00 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 50 43 42 4E 30 30 30 31 03 00 02 00 00 02 00 00 00 02 00 00 30 37 2B 00 CC 1C 2E 00", 0, ref process);
                    hextoarray("53 4D 49 4C 45 42 41 53 49 43 20 47 52 50 46 4F 4F 54 45 52", 524396, ref process);
                    Console.WriteLine("Writing image data...");
                    int y, x;
                    for (y = 0; y < 512; y++)
                    {
                        for (x = 0; x < 512; x++)
                        {
                            Color c = input.GetPixel(x, y);
                            colortoarray(c, 108 + x * 2 + y * 2 * 512, ref process);
                        }
                    }
                    File.WriteAllBytes(file + ".grp", process);
                    Console.WriteLine("Saved as \"" + file + ".grp\"");
                }
            }
        }
        static void colortoarray(Color color, int pos, ref byte[] data)
        {
            int sum = (int)((color.R * 31 / 255) * Math.Pow(2, 11) + (color.G * 31 / 255) * Math.Pow(2, 6) + (color.B * 31 / 255) * Math.Pow(2, 1) + (color.A / 255));
            data[pos] = (byte)(sum & 255);
            data[pos + 1] = (byte)((sum - (sum & 255)) / 256);
        }

        static void hextoarray(string s, int pos, ref byte[] data)
        {
            int i;
            for (i = 0; i <= s.Split(' ').Length-1; i++)
            {
                data[i + pos] = (byte)int.Parse(s.Split(' ')[i], System.Globalization.NumberStyles.HexNumber);
            }
        }
    }
}