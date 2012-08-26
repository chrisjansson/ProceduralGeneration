using OpenTK.Input;

namespace CjClutter.OpenGl.Input.Keboard
{
    public class KeyboardInputProcessor
    {
        private KeyboardState _previousKeyboardState;
        private KeyboardState _currentKeyboardState;

        public KeyboardInputProcessor()
        {
            _previousKeyboardState = new KeyboardState();
            _currentKeyboardState = new KeyboardState();
        }

        public void Update(KeyboardState keyboardState)
        {
            _previousKeyboardState = _currentKeyboardState;
            _currentKeyboardState = keyboardState;
        }

        public bool WasKeyReleased(Key key)
        {
            var previouseMouseButtonState = _previousKeyboardState[key];
            var currentMouseButtonState = _currentKeyboardState[key];

            return previouseMouseButtonState && !currentMouseButtonState;
        }

        public bool WasKeyPressed(Key key)
        {
            var previouseMouseButtonState = _previousKeyboardState[key];
            var currentMouseButtonState = _currentKeyboardState[key];

            return !previouseMouseButtonState && currentMouseButtonState;
        }

        public bool IsButtonDown(Key key)
        {
            return _currentKeyboardState.IsKeyDown(key);
        }

        public bool IsButtonUp(Key key)
        {
            return _currentKeyboardState.IsKeyUp(key);
        }
    }
}
