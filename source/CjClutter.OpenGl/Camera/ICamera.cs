using OpenTK;

namespace CjClutter.OpenGl.Camera
{
    public interface ICamera
    {
        Vector3d Position { get; set; }
        Vector3d Target { get; set; }
        Vector3d Up { get; set; }

        Matrix4d ComputeCameraMatrix();
    }
}