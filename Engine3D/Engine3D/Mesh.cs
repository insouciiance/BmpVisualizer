using System.Numerics;

namespace Engine3D
{
    public class Mesh
    {
        public Face[] Faces { get; }

        public Mesh(Face[] faces)
        {
            this.Faces = faces;
        }

        public Mesh(Vector3[] vertices, int[] triangles)
        {
            Faces = new Face[triangles.Length / 3];
            for (int i = 0; i <= triangles.Length - 3; i+=3)
            {
                Faces[i / 3] = new Face(vertices[triangles[i]], vertices[triangles[i + 1]], vertices[triangles[i + 2]]);
            }
        }
    }
}