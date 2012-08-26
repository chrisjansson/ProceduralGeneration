namespace CjClutter.OpenGl.Input.Keboard
{
    public class KeyUpActionEvaluator : KeyActionEvaluatorBase
    {
        public KeyUpActionEvaluator(KeyboardInputProcessor keyboardInputProcessor) 
            : base(keyboardInputProcessor)
        {
            KeyEvaluator = KeyboardInputProcessor.WasKeyReleased;
        }
    }
}