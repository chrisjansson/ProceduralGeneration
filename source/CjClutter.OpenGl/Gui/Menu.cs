using OpenTK;

namespace CjClutter.OpenGl.Gui
{
    public class Menu : GwenGuiBase
    {
        private readonly GenerationSettingsControl _generationSettingsControl;

        public Menu(GameWindow gameWindow)
        {
            var input = new Gwen.Input.OpenTK(gameWindow);
            input.Initialize(Root);

            _generationSettingsControl = new GenerationSettingsControl(Root);

            gameWindow.Mouse.Move += (sender, args) => input.ProcessMouseMessage(args);
            gameWindow.Mouse.ButtonDown += (sender, args) => input.ProcessMouseMessage(args);
            gameWindow.Mouse.ButtonUp += (sender, args) => input.ProcessMouseMessage(args);
            gameWindow.Mouse.WheelChanged += (sender, args) => input.ProcessMouseMessage(args);

            gameWindow.Keyboard.KeyDown += (sender, args) => input.ProcessKeyDown(args);
            gameWindow.Keyboard.KeyUp += (sender, args) => input.ProcessKeyUp(args);
        }

        public void Update()
        {
            _generationSettingsControl.Update();
        }
    }
}