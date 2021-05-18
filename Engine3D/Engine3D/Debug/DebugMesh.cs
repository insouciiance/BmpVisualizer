using System.Numerics;

namespace Engine3D.Debug
{
    public static class DebugMesh
    {
        public static Mesh Cube = new Mesh(new[]
        {
            new Vector3(0, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(1, 1, 0),
            new Vector3(0, 1, 0),
            new Vector3(0, 1, 1),
            new Vector3(1, 1, 1),
            new Vector3(1, 0, 1),
            new Vector3(0, 0, 1),
        }, new[] {
            0, 2, 1, //face front
            0, 3, 2,
            2, 3, 4, //face top
            2, 4, 5,
            1, 2, 5, //face right
            1, 5, 6,
            0, 7, 4, //face left
            0, 4, 3,
            5, 4, 7, //face back
            5, 7, 6,
            0, 6, 7, //face bottom
            0, 1, 6
        });
        public static Mesh Pyramid = new Mesh(new[]
        {
            new Vector3(0, 0, 0),
            new Vector3(0, 0, 1),
            new Vector3(1, 0, 1),
            new Vector3(1, 0, 0),
            new Vector3(.5f, 1, .5f),
        }, new[] {
            3, 1, 0, //bottom
            3, 2, 1, //bottom
            1, 4, 0, 
            2, 4, 1,
            3, 4, 2, 
            0, 4, 3,
        });
    }
}