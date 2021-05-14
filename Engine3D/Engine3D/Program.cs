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
            Stopwatch performanceTest = new Stopwatch();
            double drawTime = 0;
            double saveTime = 0;
            string url = @"C:\Users\danvu\Documents\GitHubPP\BmpVisualizer\Engine3D\Engine3D\bin\Debug\net5.0\myimg.png";
            Bitmap b = new Bitmap(1280,720);
            performanceTest.Start();
            Camera cam = new Camera(new Vector3(15, 10, -20), new Vector3(0,0,2), 2,b);
            cam.Draw();
            performanceTest.Stop();
            drawTime = performanceTest.Elapsed.TotalMilliseconds;
            performanceTest.Restart();
            b.Save(url,ImageFormat.Png);
            performanceTest.Stop();
            saveTime = performanceTest.Elapsed.TotalMilliseconds;
            
            Console.WriteLine($"Performance: draw time: {drawTime}ms  save time: {saveTime}ms");
        }
    }
}