using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Numerics;
using System.Text;

namespace Engine3D
{
    public class ConsoleRenderer
    {
        private Bitmap _image;
        
        public ConsoleRenderer(Bitmap image)
        {
            _image = image;
        }

        public void Draw()
        {
            
            Vector2 cell = new Vector2();
            for (int y = 0; y < _image.Height; y++)
            {
                for (int x = 0; x < _image.Width; x++)
                {
                    cell.X = x;
                    cell.Y = y;
                    Trace(out Color color,cell);
                    _image.SetPixel(x,y,color);
                }
            }
        }

        private void Trace(out Color color,Vector2 fracCoordinates)
        {
            Vector2 uv = (fracCoordinates - .5f*(new Vector2(_image.Width,_image.Height)))/_image.Height;
            color = Color.Black;
            Ray r = new Ray(Vector3.Zero, Vector3.Normalize(new Vector3(uv.X, uv.Y,1)));
            Vector3 sphere = new Vector3(0, 0, 8);
            Vector3 sphere2 = new Vector3(0, 0, 6);
            float radious = 1;

            float t = Vector3.Dot(sphere - r.origin, r.dir);
            Vector3 point = r.origin + r.dir * t;
            float y = (sphere - point).Length();
            if (y < radious)
            {
                float x = (float) Math.Sqrt(radious * radious - y * y);
                float t1 = t - x;
                float t2 = t + x;
                float c = Remap01(sphere.Z,sphere.Z-radious,t1);
                color = SetColor((int)(c * 255f));
            }
        }

        public float Remap01(float min,float max,float val)
        {
            return (val - min) / (max - min);
        }

        public Color SetColor(int val)
        {
            return Color.FromArgb(val, val, val);
        }
    }
}