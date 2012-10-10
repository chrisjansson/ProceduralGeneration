using OpenTK;

namespace CjClutter.OpenGl.Camera
{
    public class PerspectiveCamera : CameraBase, ICamera
    {
        public PerspectiveCamera()
        {
            Position = new Vector3d(0, 0, 2);
            Target = new Vector3d(0, 0, 0);
            Up = new Vector3d(0, 1, 0);
        }
        
        public virtual Matrix4d GetCameraMatrix()
        {
            Position.Normalize();
            Target.Normalize();
            Up.Normalize();

            return Matrix4d.LookAt(Position, Target, Up);
        }
    }
}