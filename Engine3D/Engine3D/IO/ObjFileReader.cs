using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Engine3D.IO
{
    internal class ObjFileReader : IDisposable
    {
        private readonly StreamReader _reader;

        public ObjFileReader(string path)
        {
            _reader = new StreamReader(path);
        }

        public async Task<Mesh> ReadAsync()
        {
            List<Vector3> points = new();
            List<Face> mesh = new();

            string line;

            while ((line = await _reader.ReadLineAsync()) != null)
            {
                if (line.StartsWith("v "))
                {
                    string[] splitValues = line.Remove(0, 1).Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    float[] parsedFloats = splitValues.Select((f)=>float.Parse(f, CultureInfo.InvariantCulture)).ToArray();
                    Vector3 point = new(parsedFloats[0], parsedFloats[1], parsedFloats[2]);
                    points.Add(point);
                }
                if (line.StartsWith("f "))
                {
                    string[] splitValues = line
                        .Remove(0, 1)
                        .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Split("/").First())
                        .ToArray();
                    int[] parsedVertices = splitValues.Select(int.Parse).ToArray();

                    List<Vector3> faceVertices = new();

                    foreach (int vertexIndex in parsedVertices)
                    {
                        faceVertices.Add(points[vertexIndex - 1]);
                    }

                    if (parsedVertices.Length == 3)
                    {
                        Face f = new(faceVertices.ToArray());
                        mesh.Add(f);
                    }
                    else if (parsedVertices.Length == 4)
                    {
                        Face f1 = new (new []{faceVertices[0], faceVertices[1], faceVertices[2]});
                        Face f2 = new (new []{faceVertices[0], faceVertices[2], faceVertices[3]});

                        mesh.Add(f1);
                        mesh.Add(f2);
                    }
                }
            }

            return new Mesh(mesh.ToArray());
        }

        public void Dispose() => _reader.Dispose();
    }
}
