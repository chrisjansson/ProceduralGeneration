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

        public Vector3d Position
        {
            get { return _camera.Position; }
            set { _camera.Position = value; }
        }

        public Vector3d Target
        {
            get { return _camera.Target; }
            set { _camera.Target = value; }
        }

        public Vector3d Up
        {
            get { return _camera.Up; }
            set { _camera.Up = value; }
        }

        public Matrix4d ComputeCameraMatrix()
        {
            return _camera.ComputeCameraMatrix();
        }

        public void Rotate(Vector2d startPoint, Vector2d endPoint)
        {
            var rotation = CalculateRotation(startPoint, endPoint);

            var result = Vector3d.Transform(Position, rotation);
            Position = result;
        }

        public void Zoom(double delta)
        {
            var toTarget = Target - Position;
            toTarget.Normalize();

            var newPosition = Position - (toTarget * -delta * 0.5);
            if (newPosition.Length >= MinimumDistanceToTarget)
            {
                Position = newPosition;
            }
        }

        private Quaterniond CalculateRotation(Vector2d startPoint, Vector2d endPoint)
        {
            return _cameraRotationCalculator.Calculate(startPoint, endPoint);
        }
    }
}