using OpenTK;

namespace CjClutter.OpenGl.EntityComponent
{
    public struct Bounds2D
    {
        public Bounds2D(Vector2d min, Vector2d max)
            : this()
        {
            Min = min;
            Max = max;
            Center = (Max + Min) / 2;
        }

        public Vector2d Min { get; private set; }
        public Vector2d Max { get; private set; }
        public Vector2d Center { get; private set; }

        public bool Equals(Bounds2D other)
        {
            return Min.Equals(other.Min) && Max.Equals(other.Max);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Bounds2D && Equals((Bounds2D) obj);
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