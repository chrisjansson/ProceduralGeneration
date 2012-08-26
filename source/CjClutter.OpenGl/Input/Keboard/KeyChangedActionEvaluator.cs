using OpenTK.Input;

namespace CjClutter.OpenGl.Input.Keboard
{
    public class KeyChangedActionEvaluator : KeyActionEvaluatorBase
    {
        public KeyChangedActionEvaluator(KeyboardInputProcessor keyboardInputProcessor) 
            : base(keyboardInputProcessor)
        {
            KeyEvaluator = EvaluateKey;
        }

        private bool EvaluateKey(Key key)
        {
            return KeyboardInputProcessor.WasKeyPressed(key) || KeyboardInputProcessor.WasKeyReleased(key);
        }
    }
}