using System;
using System.Drawing;
using CjClutter.ObjLoader.Viewer.Camera;
using CjClutter.OpenGl.Input.Mouse;
using OpenTK;
using OpenTK.Input;

namespace CjClutter.OpenGl.Input
{
    public class OpenTkCamera
    {
        private readonly MouseInputProcessor _mouseInputProcessor;
        private readonly ITrackballCamera _trackballCamera;
        private readonly OpenGlWindow _openGlWindow;

        private bool _mouseDown;
        private Vector2d _mouseDownPosition;
        private Vector2d _currentMousePosition;

        public OpenTkCamera(MouseInputProcessor mouseInputProcessor, ITrackballCamera trackballCamera, OpenGlWindow openGlWindow)
        {
            _mouseInputProcessor = mouseInputProcessor;
            _trackballCamera = trackballCamera;
            _openGlWindow = openGlWindow;
        }

        public void Update()
        {
            GetCurrentMousePosition();

            ProcessMouseDown();

            ProcessMouseUp();
            
            ProcessRotate();
        }

        public Matrix4d GetCameraMatrix()
        {
            return _trackballCamera.GetCameraMatrix();
        }

        private void ProcessMouseDown()
        {
            if (!_mouseDown && _mouseInputProcessor.WasButtonPressed(MouseButton.Left))
            {
                _mouseDown = true;
                _mouseDownPosition = _currentMousePosition;
            }
        }

        private void ProcessMouseUp()
        {
            if (_mouseDown && _mouseInputProcessor.WasButtonReleased(MouseButton.Left))
            {
                _mouseDown = false;
                _trackballCamera.CommitRotation(_mouseDownPosition, _currentMousePosition);
            }
        }

        private void ProcessRotate()
        {
            if (_mouseDown && (_currentMousePosition != _mouseDownPosition))
            {
                _trackballCamera.Rotate(_mouseDownPosition, _currentMousePosition);
            }
        }

        private void GetCurrentMousePosition()
        {
            var currentMousePosition = new Vector2d(_openGlWindow.Mouse.X, _openGlWindow.Mouse.Y);
            _currentMousePosition = TransformToRelative(currentMousePosition);
        }

        public Vector2d TransformToRelative(Vector2d absoluteCoordinate)
        {
            var x = absoluteCoordinate.X / _openGlWindow.Width * 2 - 1;
            var y = (_openGlWindow.Height - absoluteCoordinate.Y) / _openGlWindow.Height * 2 - 1;

            return new Vector2d(x, y);
        }
    }
}