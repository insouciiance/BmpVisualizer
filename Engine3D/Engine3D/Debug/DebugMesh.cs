using System.Numerics;

namespace Engine3D.Debug
{
    public static class DebugMesh
    {
        public static Mesh Cube = new Mesh(new[]
        {
            new Vector3(0, 0, 0),
            new Vector3(0, 0, 1),
            new Vector3(1, 0, 1),
            new Vector3(1, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(0, 1, 1),
            new Vector3(1, 1, 1),
            new Vector3(1, 1, 0),
        }, new[] {
            0, 3, 1, //face bottom
            3, 2, 1,
            4, 5, 7, //face top
            0, 4, 3,
            4, 7, 3, //face front
            1, 5, 6,
            1, 2, 5, //face back
            2, 6, 5,
            3, 7, 6, //face left
            7, 6, 2,
            0, 1, 4, //face right
            1, 5, 4
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