using OpenTK;

namespace CjClutter.OpenGl.Gui
{
    public class Menu : GwenGuiBase
    {
        private readonly GenerationSettingsControl _generationSettingsControl;
        private readonly GwenInput _input;

        public Menu(GameWindow gameWindow)
        {
            _input = new GwenInput(gameWindow);
            _input.Initialize(Root);

            _generationSettingsControl = new GenerationSettingsControl(Root);

            gameWindow.Mouse.Move += (sender, args) => _input.ProcessMouseMessage(args);
            gameWindow.Mouse.ButtonDown += (sender, args) => _input.ProcessMouseMessage(args);
            gameWindow.Mouse.ButtonUp += (sender, args) => _input.ProcessMouseMessage(args);
            gameWindow.Mouse.WheelChanged += (sender, args) => _input.ProcessMouseMessage(args);

            gameWindow.Keyboard.KeyDown += (sender, args) => _input.ProcessKeyDown(args);
            gameWindow.Keyboard.KeyUp += (sender, args) => _input.ProcessKeyUp(args);
        }

        public void Update()
        {
            _generationSettingsControl.Update();
        }
    }
}