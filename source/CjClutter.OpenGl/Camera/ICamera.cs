using OpenTK;

namespace CjClutter.OpenGl.Camera
{
    public interface ICamera
    {
        Vector3d Position { get; set; }
        Vector3d Target { get; set; }
        Vector3d Up { get; set; }
        ProjectionMode Projection { get; set; }
        double Width { get; set; }
        double Height { get; set; }
        Vector3d Forward { get; }
        Vector3d Right { get; }

        Matrix4d ComputeCameraMatrix();
        Matrix4d ComputeProjectionMatrix();
    }
}