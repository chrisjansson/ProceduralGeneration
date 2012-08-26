using OpenTK.Input;

namespace CjClutter.OpenGl.Input.Mouse
{
    public class ButtonChangedActionEvaluator : ButtonActionEvaluatorBase
    {
        public ButtonChangedActionEvaluator(MouseInputProcessor mouseInputProcessor)
            : base(mouseInputProcessor)
        {
            ButtonEvaluator = Evaluator;
        }

        private bool Evaluator(MouseButton mouseButton)
        {
            return MouseInputProcessor.WasButtonPressed(mouseButton) || MouseInputProcessor.WasButtonReleased(mouseButton);
        }
    }
}