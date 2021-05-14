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
            Camera cam = new Camera(new Vector3(0, 0, -20), new Vector3(0, 0, 1), 10,b);
            cam.Draw();
            b.Save(url,ImageFormat.Png);
        }
    }
}