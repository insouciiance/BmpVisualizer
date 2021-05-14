using System;
using System.Drawing;
using System.Numerics;

namespace Engine3D
{
    public class Camera
    {
        private Vector3 _cameraOrigin;
        private Vector3 _lookDirection;
        private float _distanceToScreen;
        private Bitmap _canvas;

        private Vector3 localRight;
        private Vector3 localUp;

        public Vector3 centerOfScreen;
        
        public Camera(Vector3 cameraOrigin, Vector3 lookDirection, float distanceToScreen,Bitmap canvas)
        {
            _cameraOrigin = cameraOrigin;
            _lookDirection = lookDirection;
            _distanceToScreen = distanceToScreen;
            _canvas = canvas;

            localRight = Vector3.Cross(lookDirection, new Vector3(0, 1, 0));
            localUp = Vector3.Cross(lookDirection, localRight);
            centerOfScreen = cameraOrigin + lookDirection * distanceToScreen;
        }

        public void Draw()
        {
            for (int y = 0; y < _canvas.Height; y++)
            {
                for (int x = 0; x < _canvas.Width; x++)
                {
                    Trace(out Color color,x,y);
                    _canvas.SetPixel(x,y,color);
                }
            }
        }

        private void Trace(out Color color,int screenX,int screenY)
        {
            Vector2 uv = (new Vector2(screenX,screenY) - .5f*(new Vector2(_canvas.Width,_canvas.Height)))/_canvas.Height;
            color = Color.Black;

            Vector3 rayInterception = centerOfScreen + uv.X * localRight + uv.Y * localUp;
            
            Ray r = new Ray(_cameraOrigin, Vector3.Normalize(rayInterception - _cameraOrigin));


            float dist = 0;
            dist += DrawPooint(r, new Vector3(0,0,0));
            dist += DrawPooint(r, new Vector3(0,0,1));
            dist += DrawPooint(r, new Vector3(0,1,0));
            dist += DrawPooint(r, new Vector3(0,1,1));
            dist += DrawPooint(r, new Vector3(1,0,0));
            dist += DrawPooint(r, new Vector3(1,0,1));
            dist += DrawPooint(r, new Vector3(1,1,0));
            dist += DrawPooint(r, new Vector3(1,1,1));

            color = Get255Color(dist);
        }

        public float DistanceToRay(Ray ray, Vector3 point)
        {
            return Vector3.Cross(point - ray.direction,ray.direction).Length() / ray.direction.Length();
        }

        public float DrawPooint(Ray ray, Vector3 point)
        {
            float dist = DistanceToRay(ray, point);
            if (dist > .1)
                return 0;

            return 1;
        }
        
        public Color Get255Color(float value)
        {
            float validC = Math.Clamp(value, 0, 1);
            if (float.IsNaN(value))
                validC = 0;
            return Color.FromArgb((int) (validC* 255), (int) (validC*255), (int)(validC*255));
        }
    }
}