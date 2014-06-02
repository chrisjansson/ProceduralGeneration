using System;
using CjClutter.OpenGl.Camera;
using CjClutter.OpenGl.Input.Keboard;
using CjClutter.OpenGl.Input.Mouse;
using OpenTK;
using OpenTK.Input;

namespace CjClutter.OpenGl.EntityComponent
{
    public class FreeCameraSystem : IEntitySystem
    {
        private readonly KeyboardInputProcessor _keyboardInputProcessor;
        private readonly ICamera _camera;
        private double _lastUpdate;
        private MouseInputProcessor _mouseInputProcessor;

        public FreeCameraSystem(KeyboardInputProcessor keyboardInputProcessor, MouseInputProcessor mouseInputProcessor, ICamera camera)
        {
            _mouseInputProcessor = mouseInputProcessor;
            _camera = camera;
            _keyboardInputProcessor = keyboardInputProcessor;
        }

        public void Update(double elapsedTime, EntityManager entityManager)
        {
            var speedExponent = Math.Max(0.5, Math.Log10(Math.Abs(_camera.Position.Y)));
            var delta = (elapsedTime - _lastUpdate) * Math.Pow(10, speedExponent);

            var forward = (_camera.Target - _camera.Position).Normalized();
            var up = _camera.Up.Normalized();
            var right = Vector3d.Cross(forward, up).Normalized();
            up = Vector3d.Cross(right, forward).Normalized();

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
                direction += up;
                keyDown = true;
            }
            if (_keyboardInputProcessor.IsButtonDown(Key.LControl))
            {
                direction += -up;
                keyDown = true;
            }

            if (keyDown)
            {
                var result = Vector3d.Multiply(direction.Normalized(), delta);
                _camera.Target += result;
                _camera.Position += result;
            }

            var relativeMousePositionDelta = _mouseInputProcessor.GetMousePositionDelta();

            var rotation = Matrix4d.CreateFromAxisAngle(up, -relativeMousePositionDelta.X / 500.0) * Matrix4d.CreateFromAxisAngle(right, -relativeMousePositionDelta.Y / 500.0);
            var rotated = Vector3d.Transform(forward, rotation);
            _camera.Target = _camera.Position + rotated;

            _lastUpdate = elapsedTime;
        }
    }
}