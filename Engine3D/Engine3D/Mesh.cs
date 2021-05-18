using System.Numerics;

namespace Engine3D
{
    public class Mesh
    {
        private Face[] faces;
        public Face[] Faces => faces;

        public Mesh(Face[] faces)
        {
            this.faces = faces;
        }

        public Mesh(Vector3[] vertices, int[] triangles)
        {
            faces = new Face[triangles.Length / 3];
            for (int i = 0; i <= triangles.Length - 3; i+=3)
            {
                faces[i / 3] = new Face(vertices[triangles[i]], vertices[triangles[i + 1]], vertices[triangles[i + 2]]);
            }
        }
    }
}