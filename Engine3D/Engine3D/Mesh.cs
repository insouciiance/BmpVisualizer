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
    }
}