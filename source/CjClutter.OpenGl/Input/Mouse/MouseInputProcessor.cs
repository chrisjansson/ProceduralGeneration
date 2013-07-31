using CjClutter.OpenGl.CoordinateSystems;
using CjClutter.OpenGl.Gui;
using OpenTK;
using OpenTK.Input;

namespace CjClutter.OpenGl.Input.Mouse
{
    public class MouseInputProcessor
    {
        private readonly GameWindow _gameWindow;
        private readonly IGuiToRelativeCoordinateTransformer _guiToRelativeCoordinateTransformer;

        private MouseState _previousFrameMouseState;
        private MouseState _currentFrameMouseState;
        private Vector2d _currentRelativeMousePosition;
        private Vector2d _previousRelativeMousePosition;

        public MouseInputProcessor(GameWindow gameWindow, IGuiToRelativeCoordinateTransformer guiToRelativeCoordinateTransformer)
        {
            _gameWindow = gameWindow;
            _guiToRelativeCoordinateTransformer = guiToRelativeCoordinateTransformer;

            var gameWindowInterfaceSizeAdapter = new GameWindowInterfaceSizeAdapter { GameWindow = gameWindow };
            _guiToRelativeCoordinateTransformer.Interface = gameWindowInterfaceSizeAdapter;

            _previousFrameMouseState = new MouseState();
            _currentFrameMouseState = new MouseState();
        }

        public void Update(MouseState mouseState)
        {
            _previousFrameMouseState = _currentFrameMouseState;
            _currentFrameMouseState = mouseState;

            CalculateRelativeMousePosition();
        }

        private void CalculateRelativeMousePosition()
        {
            var mouseDevice = _gameWindow.Mouse;

            var x = mouseDevice.X;
            var y = mouseDevice.Y;
            var absoluteMouseCoordinates = new Vector2d(x, y);

            _previousRelativeMousePosition = _currentRelativeMousePosition;
            _currentRelativeMousePosition = _guiToRelativeCoordinateTransformer.TransformToRelative(absoluteMouseCoordinates);
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

        public Vector2d GetMousePosition()
        {
            return new Vector2d(_currentFrameMouseState.X, _currentFrameMouseState.Y);
        }

        public Vector2d GetMousePositionDelta()
        {
            var x = _currentFrameMouseState.X - _previousFrameMouseState.X;
            var y = _currentFrameMouseState.Y - _previousFrameMouseState.Y;

            return new Vector2d(x, y);
        }

        public Vector2d GetRelativeMousePosition()
        {
            return _currentRelativeMousePosition;
        }

        public Vector2d GetRelativeMousePositionDelta()
        {
            return _currentRelativeMousePosition - _previousRelativeMousePosition;
        }

        public float GetMouseWheelDelta()
        {
            return _currentFrameMouseState.WheelPrecise - _previousFrameMouseState.WheelPrecise;
        }
    }
}
