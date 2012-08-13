using System;
using OpenTK.Input;

namespace CjClutter.OpenGl.Input
{
    public class MouseInputProcessor
    {
        private readonly MouseButton[] _mouseButtons;

        private MouseState _previousFrameMouseState;
        private MouseState _currentFrameMouseState;

        public MouseInputProcessor()
        {
            _mouseButtons = new[] { MouseButton.Left, MouseButton.Right };

            _previousFrameMouseState = new MouseState();
        }

        public void Update(MouseState mouseState)
        {
            _currentFrameMouseState = mouseState;

            foreach (var mouseButton in _mouseButtons)
            {
                ProcessMouseButton(mouseButton);
            }

            _previousFrameMouseState = _currentFrameMouseState;
        }

        private void ProcessMouseButton(MouseButton mouseButton)
        {
            var isButtonPressedInPreviousUpdate = _previousFrameMouseState[mouseButton];
            var isButtonPressedInCurrentUpdate = _currentFrameMouseState[mouseButton];

            var buttonStateChanged = isButtonPressedInPreviousUpdate != isButtonPressedInCurrentUpdate;

            if (buttonStateChanged && isButtonPressedInCurrentUpdate)
            {
                Console.WriteLine(mouseButton + " down.");
            }
            else if (buttonStateChanged)
            {
                Console.WriteLine(mouseButton + " up.");
            }
        }
    }
}
