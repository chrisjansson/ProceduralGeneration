using OpenTK;

namespace CjClutter.OpenGl.Camera
{
    public class LookAtCamera : ICamera
    {
        public LookAtCamera()
        {
            Position = new Vector3d(0, 0, 2);
            Target = new Vector3d(0, 0, 0);
            Up = new Vector3d(0, 1, 0);
        }

        public Vector3d Position { get; set; }
        public Vector3d Target { get; set; }
        public Vector3d Up { get; set; }

        public virtual Matrix4d GetCameraMatrix()
        {
            Position.Normalize();
            Target.Normalize();
            Up.Normalize();

            return Matrix4d.LookAt(Position, Target, Up);
        }
    }
}