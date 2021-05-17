using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Numerics;

namespace Engine3D
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch performanceTest = new ();
            string url = @"D:/my.bmp";
            Bitmap b = new (1280,720);

            performanceTest.Start();

            Camera cam = new (new Vector3(0, 0, -10), new Vector3(0, 0, 1), 1, b)
            {
                MyFace = new Face(new Vector3(0, 1, 0), new Vector3(2, -3, 0), new Vector3(-1, 0, 0))
            };
            cam.Draw();

            performanceTest.Stop();

            double drawTime = performanceTest.Elapsed.TotalMilliseconds;
            performanceTest.Restart();

            b.Save(url,ImageFormat.Png);

            performanceTest.Stop();
            double saveTime = performanceTest.Elapsed.TotalMilliseconds;
            
            Console.WriteLine($"Performance: draw time: {drawTime}ms  save time: {saveTime}ms");
        }
    }
}