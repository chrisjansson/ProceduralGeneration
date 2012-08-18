using OpenTK.Input;

namespace CjClutter.OpenGl.Input
{
    public class MouseInputProcessor
    {
        private MouseState _previousFrameMouseState;
        private MouseState _currentFrameMouseState;

        public MouseInputProcessor()
        {
            _previousFrameMouseState = new MouseState();
            _currentFrameMouseState = new MouseState();
        }

        public void Update(MouseState mouseState)
        {
            _previousFrameMouseState = _currentFrameMouseState;
            _currentFrameMouseState = mouseState;
        }
        
        public bool WasButtonReleased(MouseButton button)
        {
            var previouseMouseButtonState = _previousFrameMouseState[button];
            var currentMouseButtonState = _currentFrameMouseState[button];

            return previouseMouseButtonState && !currentMouseButtonState;
        }

        public bool WasButtonPressed(MouseButton button)
        {
            var previouseMouseButtonState = _previousFrameMouseState[button];
            var currentMouseButtonState = _currentFrameMouseState[button];

            return !previouseMouseButtonState && currentMouseButtonState;
        }

        public bool IsButtonDown(MouseButton button)
        {
            return _currentFrameMouseState.IsButtonDown(button);
        }

        public bool IsButtonUp(MouseButton button)
        {
            return _currentFrameMouseState.IsButtonUp(button);
        }
    }
}
