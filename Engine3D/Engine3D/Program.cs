﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using Engine3D.Debug;
using Engine3D.IO;

namespace Engine3D
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string meshUrl = "cow.obj";
            ObjFileReader reader = new(meshUrl);
            Mesh mesh = await reader.ReadAsync();

            Stopwatch performanceTest = new();
            string url = @"cow44k.png";
            Bitmap b = new(800, 600);

            performanceTest.Start();
            Camera cam = new(new Vector3(28, 28, 28), new Vector3(0, 0, 0), 1, b)
            {
                Mesh = mesh,
                Lights = new List<Light>{new (new Vector3(28, 28, 28), MyColor.White, 500f)}
            };
            cam.Draw();

            performanceTest.Stop();

            double drawTime = performanceTest.Elapsed.TotalMilliseconds;
            performanceTest.Restart();

            b.Save(url, ImageFormat.Png);

            performanceTest.Stop();
            double saveTime = performanceTest.Elapsed.TotalMilliseconds;

            Console.WriteLine($"Performance: draw time: {drawTime}ms  save time: {saveTime}ms");
        }
    }
}