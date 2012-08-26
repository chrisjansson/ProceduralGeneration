using System;
using OpenTK.Input;

namespace CjClutter.OpenGl.Input.Keboard
{
    public abstract class KeyActionEvaluatorBase : IKeyActionEvaluator
    {
        protected KeyboardInputProcessor KeyboardInputProcessor { get; set; }
        protected Func<Key, bool> KeyEvaluator = key => false;

        protected KeyActionEvaluatorBase(KeyboardInputProcessor keyboardInputProcessor)
        {
            KeyboardInputProcessor = keyboardInputProcessor;
        }

        public bool ShouldKeyActionBeFired(Key key)
        {
            return KeyEvaluator(key);
        }
    }
}