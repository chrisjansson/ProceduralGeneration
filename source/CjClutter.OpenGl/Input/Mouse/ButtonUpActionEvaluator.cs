namespace CjClutter.OpenGl.Input.Mouse
{
    public class ButtonUpActionEvaluator : ButtonActionEvaluatorBase
    {
        public ButtonUpActionEvaluator(MouseInputProcessor mouseInputProcessor)
            : base(mouseInputProcessor)
        {
            ButtonEvaluator = MouseInputProcessor.WasButtonReleased;
        }
    }
}