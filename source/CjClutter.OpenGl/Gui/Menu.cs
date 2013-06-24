using OpenTK;

namespace CjClutter.OpenGl.Gui
{
    public class Menu : GwenGuiBase
    {
        private readonly GenerationSettingsControl _generationSettingsControl;
        private readonly GwenInputTranslator _inputTranslator;

        public Menu(GameWindow gameWindow)
        {
            _inputTranslator = new GwenInputTranslator(gameWindow);
            _inputTranslator.Initialize(Root);

            _generationSettingsControl = new GenerationSettingsControl(Root);

            gameWindow.Mouse.Move += (sender, args) => _inputTranslator.ProcessMouseMessage(args);
            gameWindow.Mouse.ButtonDown += (sender, args) => _inputTranslator.ProcessMouseMessage(args);
            gameWindow.Mouse.ButtonUp += (sender, args) => _inputTranslator.ProcessMouseMessage(args);
            gameWindow.Mouse.WheelChanged += (sender, args) => _inputTranslator.ProcessMouseMessage(args);

            gameWindow.Keyboard.KeyDown += (sender, args) => _inputTranslator.ProcessKeyDown(args);
            gameWindow.Keyboard.KeyUp += (sender, args) => _inputTranslator.ProcessKeyUp(args);
        }

        public void Update()
        {
            _generationSettingsControl.Update();
        }
    }
}