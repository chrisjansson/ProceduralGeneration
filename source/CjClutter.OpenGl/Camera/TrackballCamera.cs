using CjClutter.OpenGl.OpenTk;
using OpenTK;

namespace CjClutter.OpenGl.Camera
{
    public class TrackballCamera : ITrackballCamera
    {
        const double MinimumDistanceToTarget = 0.2;

        private readonly ICamera _camera;
        private readonly ITrackballCameraRotationCalculator _cameraRotationCalculator;

        private Matrix4d _cameraOrientation = Matrix4d.Identity;
        private Matrix4d _tempCameraOrientation = Matrix4d.Identity;

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

        public Matrix4d GetCameraMatrix()
        {
            var cameraMatrix = _camera.GetCameraMatrix();
            var orientationMatrix = GetOrientationMatrix();

            return orientationMatrix.Multiply(cameraMatrix);
        }

        public void Rotate(Vector2d startPoint, Vector2d endPoint)
        {
            Quaterniond rotation = CalculateRotation(startPoint, endPoint);
            _tempCameraOrientation = rotation.GetRotationMatrix();
        }

        public void CommitRotation(Vector2d startPoint, Vector2d endPoint)
        {
            Quaterniond rotation = CalculateRotation(startPoint, endPoint);
            Matrix4d rotationMatrix = rotation.GetRotationMatrix();

            _cameraOrientation = _cameraOrientation.Multiply(rotationMatrix);
            _tempCameraOrientation = Matrix4d.Identity;
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

        private Matrix4d GetOrientationMatrix()
        {
            return _cameraOrientation.Multiply(_tempCameraOrientation);
        }

        private Quaterniond CalculateRotation(Vector2d startPoint, Vector2d endPoint)
        {
            return _cameraRotationCalculator.Calculate(startPoint, endPoint);
        }
    }
}