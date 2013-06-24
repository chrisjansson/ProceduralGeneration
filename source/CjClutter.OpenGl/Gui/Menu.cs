using OpenTK;
using OpenTK.Input;

namespace CjClutter.OpenGl.Gui
{
    public class Menu : GwenGuiBase
    {
        private readonly GameWindow _gameWindow;
        private readonly GwenInputTranslator _inputTranslator;
        private readonly GenerationSettingsControl _generationSettingsControl;

        public Menu(GameWindow gameWindow)
        {
            _gameWindow = gameWindow;
            _inputTranslator = new GwenInputTranslator(Root);

            _generationSettingsControl = new GenerationSettingsControl(Root);
            Enable();
        }

        public void Enable()
        {
            _gameWindow.Mouse.Move += OnMouseOnMove;
            _gameWindow.Mouse.ButtonDown += OnMouseOnButtonDown;
            _gameWindow.Mouse.ButtonUp += OnMouseOnButtonUp;
            _gameWindow.Mouse.WheelChanged += OnMouseOnWheelChanged;

            _gameWindow.Keyboard.KeyDown += OnKeyboardOnKeyDown;
            _gameWindow.Keyboard.KeyUp += OnKeyboardOnKeyUp;
            _gameWindow.KeyPress += OnGameWindowOnKeyPress;
        }

        public void Disable()
        {
            _gameWindow.Mouse.Move -= OnMouseOnMove;
            _gameWindow.Mouse.ButtonDown -= OnMouseOnButtonDown;
            _gameWindow.Mouse.ButtonUp -= OnMouseOnButtonUp;
            _gameWindow.Mouse.WheelChanged -= OnMouseOnWheelChanged;

            _gameWindow.Keyboard.KeyDown -= OnKeyboardOnKeyDown;
            _gameWindow.Keyboard.KeyUp -= OnKeyboardOnKeyUp;
            _gameWindow.KeyPress -= OnGameWindowOnKeyPress;
        }

        private void OnMouseOnMove(object sender, MouseMoveEventArgs args)
        {
            _inputTranslator.ProcessMouseMessage(args);
        }

        public void Update()
        {
            _generationSettingsControl.Update();
        }

        private void OnMouseOnButtonDown(object sender, MouseButtonEventArgs args)
        {
            _inputTranslator.ProcessMouseMessage(args);
        }

        private void OnMouseOnButtonUp(object sender, MouseButtonEventArgs args)
        {
            _inputTranslator.ProcessMouseMessage(args);
        }

        private void OnMouseOnWheelChanged(object sender, MouseWheelEventArgs args)
        {
            _inputTranslator.ProcessMouseMessage(args);
        }

        private void OnKeyboardOnKeyDown(object sender, KeyboardKeyEventArgs args)
        {
            _inputTranslator.ProcessKeyDown(args);
        }

        private void OnKeyboardOnKeyUp(object sender, KeyboardKeyEventArgs args)
        {
            _inputTranslator.ProcessKeyUp(args);
        }

        private void OnGameWindowOnKeyPress(object sender, KeyPressEventArgs args)
        {
            _inputTranslator.KeyPress(sender, args);
        }
    }
}