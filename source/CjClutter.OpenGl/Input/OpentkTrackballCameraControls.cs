using CjClutter.OpenGl.Camera;
using CjClutter.OpenGl.Input.Mouse;
using OpenTK;
using OpenTK.Input;

namespace CjClutter.OpenGl.Input
{
    public class OpentkTrackballCameraControls
    {
        private readonly MouseInputProcessor _mouseInputProcessor;
        private readonly ITrackballCamera _trackballCamera;

        public OpentkTrackballCameraControls(MouseInputProcessor mouseInputProcessor, ITrackballCamera trackballCamera)
        {
            _mouseInputProcessor = mouseInputProcessor;
            _trackballCamera = trackballCamera;
        }

        public void Update()
        {
            ProcessRotatation();
            
            ProcessScroll();
        }

        private void ProcessRotatation()
        {
            var relativeMousePositionDelta = _mouseInputProcessor.GetRelativeMousePositionDelta();
            if (_mouseInputProcessor.IsButtonDown(MouseButton.Left) && relativeMousePositionDelta.Length != 0)
            {
                _trackballCamera.Rotate(relativeMousePositionDelta, Vector2d.Zero);
            }
        }

        private void ProcessScroll()
        {
            var mouseWheelDelta = _mouseInputProcessor.GetMouseWheelDelta();
            if (mouseWheelDelta != 0)
            {
                _trackballCamera.Zoom(mouseWheelDelta);
            }
        }
    }
}