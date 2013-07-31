using OpenTK;

namespace CjClutter.OpenGl.Camera
{
    public interface ITrackballCamera : ICamera
    {
        void Rotate(Vector2d startPoint, Vector2d endPoint);
        void Zoom(double delta);
    }
}