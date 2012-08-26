using System;
using OpenTK.Input;

namespace CjClutter.OpenGl.Input.Mouse
{
    public abstract class ButtonActionEvaluatorBase : IButtonActionEvaluator
    {
        protected MouseInputProcessor MouseInputProcessor { get; set; }
        protected Func<MouseButton, bool> ButtonEvaluator = button => false;

        protected ButtonActionEvaluatorBase(MouseInputProcessor mouseInputProcessor)
        {
            MouseInputProcessor = mouseInputProcessor;
        }

        public bool ShouldButtonActionBeFired(MouseButton button)
        {
            return ButtonEvaluator(button);
        }
    }
}