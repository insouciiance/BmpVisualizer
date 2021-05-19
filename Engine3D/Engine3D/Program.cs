using System;
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
        
        /*
         * Todo Fix camera uv + rotation
         * Todo Fix looking for interception code
         * 
         */
        static async Task Main(string[] args)
        {
            string meshUrl = "cow.obj";
            ObjFileReader reader = new(meshUrl);
            Mesh mesh = await reader.ReadAsync();

            Stopwatch performanceTest = new();
            string url = @"D:/renders/my15.png";
            Bitmap b = new(320, 180);

            performanceTest.Start();

            Camera cam = new(new Vector3(0, -1, 0), new Vector3(0, 0, 0), 1, b)
            {
                Mesh = mesh,
                Lights = new List<Light>{new (new Vector3(0, -1, 0), MyColor.White, .5f)}
            };
            cam.Draw();

            performanceTest.Stop();

            double drawTime = performanceTest.Elapsed.TotalMilliseconds;
            performanceTest.Restart();

            b.Save(url, ImageFormat.Png);

            performanceTest.Stop();
            double saveTime = performanceTest.Elapsed.TotalMilliseconds;

            Console.WriteLine($"Performance: draw time: {drawTime}ms  save time: {saveTime}ms");
            
            
            //RandomCordTest(@"imgs/myimg",DebugMesh.Pyramid,20,3);
        }

        /*public static void RandomCordTest(string baseFileName,Mesh mesh,int count,float maxDist = 10,float minDist = 1f)
        {
            Random rnd = new Random();
            Vector3 pos = Vector3.One;
            for (int i = 0; i < count; i++)
            {
                pos = rnd.IncidentSphere() * (float) rnd.NextDouble(minDist,maxDist);
                Bitmap b = new(720, 360);
                Camera cam = new(pos, new Vector3(0, 0, 0), .4f, b)
                {
                    Mesh = mesh,
                    Light = new Vector3(1,2,0)
                    /*Lights = new List<Light>
                    {
                        new(new Vector3(0,5,1),MyColor.Red,50),
                        new(new Vector3(2,2,5),MyColor.Green,50),
                        new(new Vector3(0,-5,1),MyColor.Blue,50),
                    }#1#
                };
                cam.Draw();
                Console.WriteLine($"Frame: {i} finish saving it. {count - i - 1} frames to go");
                b.Save(baseFileName + "_" + i + ".png",ImageFormat.Png);
            }
        }*/
    }
}