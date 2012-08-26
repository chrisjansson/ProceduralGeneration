namespace CjClutter.OpenGl.Input.Keboard
{
    public class KeyDownActionEvaluator : KeyActionEvaluatorBase
    {
        public KeyDownActionEvaluator(KeyboardInputProcessor keyboardInputProcessor) 
            : base(keyboardInputProcessor)
        {
            KeyEvaluator = keyboardInputProcessor.WasKeyPressed;
        }
    }
}