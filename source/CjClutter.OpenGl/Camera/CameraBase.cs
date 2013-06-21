using OpenTK;

namespace CjClutter.OpenGl.Camera
{
    public class CameraBase
    {
        public Vector3d Position { get; set; }
        public Vector3d Target { get; set; }
        public Vector3d Up { get; set; }
    }
}