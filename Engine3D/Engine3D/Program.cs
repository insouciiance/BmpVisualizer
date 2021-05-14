using System;
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
            string url = @"C:\Users\danvu\Documents\GitHubPP\BmpVisualizer\Engine3D\Engine3D\bin\Debug\net5.0\myimg.png";
            Bitmap b = new Bitmap(1280,720);
            ConsoleRenderer renderer = new ConsoleRenderer(b);
            renderer.Draw();
            b.Save(url,ImageFormat.Png);
        }
    }
}