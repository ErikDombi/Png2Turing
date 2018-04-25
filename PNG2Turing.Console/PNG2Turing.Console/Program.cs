using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

namespace PNG2Turing.Console
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {

            System.Console.ForegroundColor = ConsoleColor.White;
            System.Console.WriteLine("Image2Turing Created by Erik Dombi");
            if (!File.Exists("image.png"))
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine("Failed to find image.png! Create it and try again!");
                System.Console.ReadLine();
                return;
            }
            try
            {
                System.Console.WriteLine("Please enter a skip value (Lower skip value = better quality, but takes longer to convert)");
                System.Console.ForegroundColor = ConsoleColor.Yellow;
                System.Console.Write(" > ");
                System.Console.ForegroundColor = ConsoleColor.White;
                int skip = int.Parse(System.Console.ReadLine());
                System.Console.Clear();
                System.Console.WriteLine("Please enter a color range value (Higher = Better colours)");
                System.Console.ForegroundColor = ConsoleColor.Yellow;
                System.Console.Write(" > ");
                System.Console.ForegroundColor = ConsoleColor.White;
                int contrast = int.Parse(System.Console.ReadLine());
                System.Console.WriteLine("Starting...\n");
                Stopwatch sw = new Stopwatch();
                sw.Start();

                Bitmap bmp = Image.FromFile("image.png") as Bitmap;

                bmp.RotateFlip(RotateFlipType.Rotate270FlipY);

                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);

                BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);


                // Get the address of the first line.
                IntPtr ptr = bmpData.Scan0;

                // Declare an array to hold the bytes of the bitmap.
                int bytes = bmpData.Stride * bmp.Height;
                byte[] rgbValues = new byte[bytes];

                Marshal.Copy(ptr, rgbValues, 0, bytes);

                int stride = bmpData.Stride;

                string code = "";
                code += $"import GUI\nsetscreen(\"graphics:{bmp.Height};{bmp.Width}\")\n";
                double maxPix = 0;
                for (int column = 0; column < bmp.Height; column += skip)
                {
                    for (int row = 0; row < bmp.Width; row += skip)
                    {
                        code += $"Draw.FillBox ({column}, {bmp.Width - row}, {column + skip}, {bmp.Width - row + skip}, RGB.AddColor({Math.Round((byte)(rgbValues[(column * stride) + (row * 3) + 2]) * 0.392156862745098 / 100, contrast)}, {Math.Round((byte)(rgbValues[(column * stride) + (row * 3) + 1]) * 0.392156862745098 / 100, contrast)}, {Math.Round((byte)(rgbValues[(column * stride) + (row * 3)]) * 0.392156862745098 / 100, contrast)}))\n";
                        if (column * row > maxPix) maxPix = column * row;
                        double prog = (double)((double)maxPix) / (double)((double)bmp.Width * (double)bmp.Height) * 100;
                        System.Console.Write($"\r[{Math.Round(prog).ToString()}%] {column}, {row}");
                    }
                }

                Clipboard.SetText(code);
                sw.Stop();
                System.Console.ForegroundColor = ConsoleColor.Green;
                System.Console.WriteLine($"\nDone! Completed task in {sw.ElapsedMilliseconds / 1000} seconds!");
                System.Console.ForegroundColor = ConsoleColor.White;
                System.Console.WriteLine("Copied output to clipboard! Press enter to exit!");
                System.Console.ReadLine();
            }
            catch (Exception ex)
            {
                System.Console.Clear();
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine($"A fatal error occured:\n{ex.Message}");
                System.Console.ReadLine();
                System.Console.Clear();
                Main(null);
            }

        }
    }
}
