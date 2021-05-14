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
        
        public Camera(Vector3 cameraOrigin, Vector3 lookAtPoint, float distanceToScreen,Bitmap canvas)
        {
            _cameraOrigin = cameraOrigin;
            _distanceToScreen = distanceToScreen;
            _canvas = canvas;

            _lookDirection = Vector3.Normalize(lookAtPoint - _cameraOrigin);
            localRight = Vector3.Cross(_lookDirection, new Vector3(0, 1, 0));
            localUp = Vector3.Cross(_lookDirection, localRight);
            centerOfScreen = cameraOrigin + _lookDirection * distanceToScreen;
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
            float radious = .5f;
            dist += DrawDebugSphere(r, new Vector3(0, 0, 2), radious);
            dist += DrawDebugSphere(r, new Vector3(0, 0, 4), radious);
            dist += DrawDebugSphere(r, new Vector3(0, 2, 2), radious);
            dist += DrawDebugSphere(r, new Vector3(0, 2, 4), radious);
            dist += DrawDebugSphere(r, new Vector3(2, 0, 2), radious);
            dist += DrawDebugSphere(r, new Vector3(2, 0, 4), radious);
            dist += DrawDebugSphere(r, new Vector3(2, 2, 2), radious);
            dist += DrawDebugSphere(r, new Vector3(2, 2, 4), radious);

            color = Get255Color(dist);
        }

        public float DistanceToRay(Ray ray, Vector3 point)
        {
            return Vector3.Cross(point - ray.direction,ray.direction).Length() / ray.direction.Length();
        }

        public float DrawPooint(Ray ray, Vector3 point)
        {
            float dist = DistanceToRay(ray, point);
            if (dist > 8)
                return 0;

            return 1;
        }

        public float DrawDebugSphere(Ray ray,Vector3 center,float radious)
        {
            float t = Vector3.Dot(center - ray.origin, ray.direction);
            Vector3 point = ray.origin + ray.direction * t;

            float y = (center - point).Length();
            if (y < radious)
            {
                return 1;
            }

            return 0;
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