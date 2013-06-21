using CjClutter.OpenGl.Camera;
using CjClutter.OpenGl.Input.Mouse;
using OpenTK;
using OpenTK.Input;

namespace CjClutter.OpenGl.Input
{
    public class OpenTkCamera
    {
        private readonly MouseInputProcessor _mouseInputProcessor;
        private readonly ITrackballCamera _trackballCamera;

        private bool _mouseDown;
        private Vector2d _mouseDownPosition;
        private Vector2d _currentMousePosition;

        public OpenTkCamera(MouseInputProcessor mouseInputProcessor, ITrackballCamera trackballCamera)
        {
            _mouseInputProcessor = mouseInputProcessor;
            _trackballCamera = trackballCamera;
        }

        public void Update()
        {
            GetCurrentMousePosition();

            ProcessMouseDown();

            ProcessMouseUp();
            
            ProcessRotate();
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
            var relativeMousePosition = _mouseInputProcessor.GetRelativeMousePosition();
            _currentMousePosition = relativeMousePosition;
        }
    }
}