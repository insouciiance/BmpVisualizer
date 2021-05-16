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

        public Mesh mesh;
        public Face MyFace;
        
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

            Vector3 rayInterception = centerOfScreen + uv.X * localRight + -uv.Y * localUp;
            
            Ray r = new Ray(_cameraOrigin, Vector3.Normalize(rayInterception - _cameraOrigin));

            

            /*
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

            color = Get255Color(dist);*/
            color = Get255Color(RayFaceInterseption(r, MyFace, out float t) ? 1 : 0);
            // uv = new Vector2(screenX, screenY) / new Vector2(_canvas.Width, _canvas.Height);
             //color = Get255Color(uv.X,uv.Y);
        }

        public float DistanceToRay(Ray ray, Vector3 point)
        {
            return Vector3.Cross(point - ray.dir,ray.dir).Length() / ray.dir.Length();
        }

        public bool RayFaceInterseption(Ray ray, Face face,out float t)
        {
            Vector3 faceNormal = GetPlaneNormal(face.Points[0], face.Points[1], face.Points[2]);
            faceNormal = Vector3.Normalize(faceNormal);
            // check if ray and plane are parallel ?
            t = -1;
            float NdotRayDirection = Vector3.Dot(faceNormal,ray.dir); 
            if (Math.Abs(NdotRayDirection) < .01f) // almost 0 
                return false; // they are parallel so they don't intersect !
            
            // compute d parameter using equation 2
            float d = Vector3.Dot(faceNormal,face.Points[0]); 
          //  Console.WriteLine("d");
            // compute t (equation 3)
            t = (Vector3.Dot(faceNormal,ray.origin) + d) / NdotRayDirection; 
            // check if the triangle is in behind the ray
           // if (t < 0) return false; // the triangle is behind 
 
            // compute the intersection point using equation 1
            Vector3 P = ray.origin + t * ray.dir; 
 
            // Step 2: inside-outside test
            Vector3 C; // vector perpendicular to triangle's plane 
           // Console.WriteLine("d1");
            // edge 0
            Vector3 edge0 = face.Points[1] - face.Points[0]; 
            Vector3 vp0 = P - face.Points[0]; 
            C = Vector3.Cross(edge0,vp0);
            if (Vector3.Dot(faceNormal,C) < 0) return false; // P is on the right side 
           // Console.WriteLine("d");
            // edge 1
            Vector3 edge1 = face.Points[2] - face.Points[1]; 
            Vector3 vp1 = P - face.Points[1]; 
            C = Vector3.Cross(edge1,vp1);
            if (Vector3.Dot(faceNormal,C) < 0)  return false; // P is on the right side 
           // Console.WriteLine("d");
            // edge 2
            Vector3 edge2 = face.Points[0] - face.Points[2]; 
            Vector3 vp2 = P - face.Points[2]; 
            C = Vector3.Cross(edge2,vp2);
            if (Vector3.Dot(faceNormal,C) < 0) return false; // P is on the right side; 
 
            return true; // this ray hits the triangle 
            
        }

        public Vector3 GetPlaneNormal(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            Vector3 dir1 = p2 - p1; 
            Vector3 dir2 = p3 - p1;
            return Vector3.Cross(dir1,dir2);
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
            float t = Vector3.Dot(center - ray.origin, ray.dir);
            Vector3 point = ray.origin + ray.dir * t;

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
        public Color Get255Color(float r,float g)
        {
            float vR = Math.Clamp(r, 0, 1);
            if (float.IsNaN(r))
                vR = 0;
            float vG = Math.Clamp(g, 0, 1);
            if (float.IsNaN(g))
                vG = 0;
            return Color.FromArgb((int) (vR* 255), (int) (vG*255), 0);
        }
    }
}