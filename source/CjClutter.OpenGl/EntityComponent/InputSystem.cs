using CjClutter.OpenGl.Camera;
using CjClutter.OpenGl.Input.Keboard;
using CjClutter.OpenGl.OpenTk;
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

        private double _lastUpdate = 0;
        private readonly ICamera _camera;

        public void Update(double elapsedTime, EntityManager entityManager)
        {
            var delta = elapsedTime - _lastUpdate;

            var forward = _camera.Target - _camera.Position;
            forward.Normalize();
            var up = _camera.Up;
            up.Normalize();

            var right = Vector3d.Cross(forward, up);
            right.Normalize();

            if (_keyboardInputProcessor.IsButtonDown(Key.W))
            {
                var result = Vector3d.Multiply(forward, delta);
                _camera.Target += result;
                _camera.Position += result;
            }
            if (_keyboardInputProcessor.IsButtonDown(Key.S))
            {
                var result = Vector3d.Multiply(-forward, delta);
                _camera.Target += result;
                _camera.Position += result;
            }
            if (_keyboardInputProcessor.IsButtonDown(Key.D))
            {
                var result = Vector3d.Multiply(right, delta);
                _camera.Target += result;
                _camera.Position += result;
            }
            if (_keyboardInputProcessor.IsButtonDown(Key.A))
            {
                var result = Vector3d.Multiply(-right, delta);
                _camera.Target += result;
                _camera.Position += result;
            }

            if (_keyboardInputProcessor.IsButtonDown(Key.E))
            {
                var rotation = Matrix4d.Rotate(forward, delta);
                _camera.Target = Vector3d.Transform(_camera.Target, rotation);
                _camera.Position = Vector3d.Transform(_camera.Position, rotation);
                _camera.Up = Vector3d.Transform(_camera.Up, rotation);
            }

            if (_keyboardInputProcessor.IsButtonDown(Key.Q))
            {
                var rotation = Matrix4d.Rotate(forward, -delta);
                _camera.Target = Vector3d.Transform(_camera.Target, rotation);
                _camera.Position = Vector3d.Transform(_camera.Position, rotation);
                _camera.Up = Vector3d.Transform(_camera.Up, rotation);
            }

            _lastUpdate = elapsedTime;
        }
    }
}