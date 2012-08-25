using System;
using OpenTK.Input;

namespace CjClutter.OpenGl.Input
{
    public interface IButtonEventEvaluator
    {
        bool ShouldButtonActionBeFired(MouseButton button);
    }

    public abstract class ButtonActionEvaluatorBase : IButtonEventEvaluator
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

    public class ButtonUpActionEvaluator : ButtonActionEvaluatorBase
    {
        public ButtonUpActionEvaluator(MouseInputProcessor mouseInputProcessor)
            : base(mouseInputProcessor)
        {
            ButtonEvaluator = MouseInputProcessor.WasButtonReleased;
        }
    }

    public class ButtonDownActionEvaluator : ButtonActionEvaluatorBase
    {
        public ButtonDownActionEvaluator(MouseInputProcessor mouseInputProcessor)
            : base(mouseInputProcessor)
        {
            ButtonEvaluator = MouseInputProcessor.WasButtonPressed;
        }
    }
}