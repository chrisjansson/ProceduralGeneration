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

        public bool Equals(Box3D other)
        {
            return Min.Equals(other.Min) && Max.Equals(other.Max);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Box3D && Equals((Box3D) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Min.GetHashCode()*397) ^ Max.GetHashCode();
            }
        }
    }
}