namespace CjClutter.OpenGl.Input.Mouse
{
    public class ButtonDownActionEvaluator : ButtonActionEvaluatorBase
    {
        public ButtonDownActionEvaluator(MouseInputProcessor mouseInputProcessor)
            : base(mouseInputProcessor)
        {
            ButtonEvaluator = MouseInputProcessor.WasButtonPressed;
        }
    }
}