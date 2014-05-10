using CjClutter.OpenGl.Camera;
using CjClutter.OpenGl.Input.Keboard;
using OpenTK;
using OpenTK.Input;

namespace CjClutter.OpenGl.EntityComponent
{
    public class InputSystem : IEntitySystem
    {
        private readonly KeyboardInputProcessor _keyboardInputProcessor;

        public InputSystem(KeyboardInputProcessor keyboardInputProcessor, ICamera camera)
        {
            _camera = camera;
            _keyboardInputProcessor = keyboardInputProcessor;
        }

        private double _lastUpdate;
        private readonly ICamera _camera;

        public void Update(double elapsedTime, EntityManager entityManager)
        {
            var delta = (elapsedTime - _lastUpdate)  * 5;

            var forward = _camera.Target - _camera.Position;
            forward.Normalize();
            var up = _camera.Up;
            up.Normalize();

            var right = Vector3d.Cross(forward, up);
            right.Normalize();

            if (_keyboardInputProcessor.IsButtonDown(Key.W))
            {
                var result = Vector3d.Multiply(new Vector3d(forward.X, 0, forward.Z), delta);
                _camera.Target += result;
                _camera.Position += result;
            }
            if (_keyboardInputProcessor.IsButtonDown(Key.S))
            {
                var result = Vector3d.Multiply(new Vector3d(-forward.X, 0, -forward.Z), delta);
                _camera.Target += result;
                _camera.Position += result;
            }
            if (_keyboardInputProcessor.IsButtonDown(Key.D))
            {
                var result = Vector3d.Multiply(new Vector3d(right.X, 0, right.Z), delta);
                _camera.Target += result;
                _camera.Position += result;
            }
            if (_keyboardInputProcessor.IsButtonDown(Key.A))
            {
                var result = Vector3d.Multiply(new Vector3d(-right.X, 0, -right.Z), delta);
                _camera.Target += result;
                _camera.Position += result;
            }

            if (_keyboardInputProcessor.IsButtonDown(Key.E))
            {
                var rotation = Matrix4d.Rotate(new Vector3d(0, 1, 0), -delta * 0.2);
                _camera.Position = _camera.Target + Vector3d.Transform(_camera.Position - _camera.Target, rotation);
            }

            if (_keyboardInputProcessor.IsButtonDown(Key.Q))
            {
                var rotation = Matrix4d.Rotate(new Vector3d(0, 1, 0), delta * 0.2);
                _camera.Position = _camera.Target + Vector3d.Transform(_camera.Position - _camera.Target, rotation);
            }

            if (_keyboardInputProcessor.IsButtonDown(Key.LShift))
            {
                _camera.Position += Vector3d.Multiply(new Vector3d(0, 1, 0), delta);
            }

            if (_keyboardInputProcessor.IsButtonDown(Key.LControl))
            {
                _camera.Position += Vector3d.Multiply(new Vector3d(0, 1, 0), -delta);
            }

            _lastUpdate = elapsedTime;
        }
    }
}