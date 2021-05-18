using System;
using System.Numerics;

namespace Engine3D.Debug
{
    public static class VectorsHelper
    {
        public static Vector3 IncidentSphere(this Random rnd)
            => Vector3.Normalize(
                new Vector3( 
                    (float) rnd.NextDouble(-1,1),
                    (float) rnd.NextDouble(-1,1),
                    (float) rnd.NextDouble(-1,1)
                    )
                );
        public static double NextDouble(this Random rnd,double minimum, double maximum)
        {
            return rnd.NextDouble() * (maximum - minimum) + minimum;
        }
    }
}