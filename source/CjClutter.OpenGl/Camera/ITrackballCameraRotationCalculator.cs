using OpenTK;

namespace CjClutter.OpenGl.Camera
{
    public interface ITrackballCameraRotationCalculator
    {
        Quaterniond Calculate(Vector2d startPoint, Vector2d endPoint);
    }
}