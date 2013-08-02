using OpenTK;

namespace CjClutter.OpenGl.Camera
{
    public class TrackballCamera : ITrackballCamera
    {
        const double MinimumDistanceToTarget = 0.2;

        private readonly ICamera _camera;
        private readonly ITrackballCameraRotationCalculator _cameraRotationCalculator;

        public TrackballCamera(ICamera camera, ITrackballCameraRotationCalculator cameraRotationCalculator)
        {
            _camera = camera;
            _cameraRotationCalculator = cameraRotationCalculator;
        }

        public Matrix4d ComputeCameraMatrix()
        {
            return _camera.ComputeCameraMatrix();
        }

        public void Rotate(Vector2d startPoint, Vector2d endPoint)
        {
            var rotation = CalculateRotation(startPoint, endPoint);

            var result = Vector3d.Transform(_camera.Position, rotation);
            _camera.Position = result;
        }

        public void Zoom(double delta)
        {
            var toTarget = _camera.Target - _camera.Position;
            toTarget.Normalize();

            var newPosition = _camera.Position - (toTarget * -delta * 0.5);
            if (newPosition.Length >= MinimumDistanceToTarget)
            {
                _camera.Position = newPosition;
            }
        }

        private Quaterniond CalculateRotation(Vector2d startPoint, Vector2d endPoint)
        {
            return _cameraRotationCalculator.Calculate(startPoint, endPoint);
        }
    }
}