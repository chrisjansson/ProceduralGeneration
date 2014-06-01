using System;
using CjClutter.OpenGl.Camera;
using CjClutter.OpenGl.Input.Keboard;
using OpenTK;
using OpenTK.Input;

namespace CjClutter.OpenGl.EntityComponent
{
    public class FreeCameraSystem : IEntitySystem
    {
        private KeyboardInputProcessor _keyboardInputProcessor;
        private ICamera _camera;
        private double _lastUpdate;

        public FreeCameraSystem(KeyboardInputProcessor keyboardInputProcessor, ICamera camera)
        {
            _camera = camera;
            _keyboardInputProcessor = keyboardInputProcessor;
        }

        public void Update(double elapsedTime, EntityManager entityManager)
        {
            var speedExponent = Math.Log10(_camera.Position.Y);

            var delta = (elapsedTime - _lastUpdate) * Math.Pow(10, speedExponent);

            var forward = (_camera.Target - _camera.Position).Normalized();
            var up = _camera.Up.Normalized();
            var right = Vector3d.Cross(forward, up).Normalized();

            var direction = new Vector3d(0, 0, 0);
            bool keyDown = false;
            if (_keyboardInputProcessor.IsButtonDown(Key.W))
            {
                direction += forward;
                keyDown = true;
            }
            if (_keyboardInputProcessor.IsButtonDown(Key.S))
            {
                direction += -forward;
                keyDown = true;
            }
            if (_keyboardInputProcessor.IsButtonDown(Key.D))
            {
                direction += right;
                keyDown = true;
            }
            if (_keyboardInputProcessor.IsButtonDown(Key.A))
            {
                direction += -right;
                keyDown = true;
            }
            if (_keyboardInputProcessor.IsButtonDown(Key.LShift))
            {
                direction += new Vector3d(0, 1, 0);
                keyDown = true;
            }
            if (_keyboardInputProcessor.IsButtonDown(Key.LControl))
            {
                direction += new Vector3d(0, -1, 0);
                keyDown = true;
            }

            if (keyDown)
            {
                var result = Vector3d.Multiply(direction.Normalized(), delta);
                _camera.Target += result;
                _camera.Position += result;

            }

            _lastUpdate = elapsedTime;
        }
    }
}