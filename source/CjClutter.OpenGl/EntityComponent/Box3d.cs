using OpenTK;

namespace CjClutter.OpenGl.EntityComponent
{
    public struct Box3D
    {
        public Box3D(Vector3d min, Vector3d max)
            : this()
        {
            Min = min;
            Max = max;
            Center = (Max + Min) / 2;
        }

        public Vector3d Min { get; private set; }
        public Vector3d Max { get; private set; }
        public Vector3d Center { get; private set; }
    }
}