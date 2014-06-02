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
        private readonly MouseInputProcessor _mouseInputProcessor;

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

            var up = Vector3d.Cross(_camera.Right, _camera.Forward).Normalized();

            var direction = new Vector3d(0, 0, 0);
            bool keyDown = false;
            if (_keyboardInputProcessor.IsButtonDown(Key.W))
            {
                direction += _camera.Forward;
                keyDown = true;
            }
            if (_keyboardInputProcessor.IsButtonDown(Key.S))
            {
                direction += -_camera.Forward;
                keyDown = true;
            }
            if (_keyboardInputProcessor.IsButtonDown(Key.D))
            {
                direction += _camera.Right;
                keyDown = true;
            }
            if (_keyboardInputProcessor.IsButtonDown(Key.A))
            {
                direction += -_camera.Right;
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
            var rotation = Matrix4d.CreateFromAxisAngle(up, -relativeMousePositionDelta.X / 500.0) * Matrix4d.CreateFromAxisAngle(_camera.Right, -relativeMousePositionDelta.Y / 500.0);
            var rotated = Vector3d.Transform(_camera.Forward, rotation);
            _camera.Target = _camera.Position + rotated;

            _lastUpdate = elapsedTime;
        }
    }
}