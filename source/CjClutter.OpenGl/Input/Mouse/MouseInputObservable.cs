using System;
using OpenTK.Input;

namespace CjClutter.OpenGl.Input.Mouse
{
    public class MouseInputObservable
    {
        private readonly IButtonActionEvaluator _actionEvaluator;

        private readonly MultiValueDictionary<MouseButton, Action> _mouseButtonDictionary;

        public MouseInputObservable(IButtonActionEvaluator actionEvaluator)
        {
            _actionEvaluator = actionEvaluator;

            var delegateComparer = new DelegateComparer();
            _mouseButtonDictionary = new MultiValueDictionary<MouseButton, Action>(delegateComparer);
        }

        public void ProcessMouseButtons()
        {
            foreach (var mouseButton in _mouseButtonDictionary.Keys)
            {
                ProcessMouseButton(mouseButton);
            }
        }

        private void ProcessMouseButton(MouseButton mouseButton)
        {
            if(_actionEvaluator.ShouldButtonActionBeFired(mouseButton))
            {
                FireMouseAction(mouseButton);
            }
        }

        private void FireMouseAction(MouseButton mouseButton)
        {
            var mouseButtonActions = _mouseButtonDictionary[mouseButton];
            foreach (var mouseButtonAction in mouseButtonActions)
            {
                mouseButtonAction();
            }
        }

        public void SubscribeMouseButton(MouseButton mouseButton, Action action)
        {
            _mouseButtonDictionary.Add(mouseButton, action);
        }

        public void UnsubscribeMouseButton(MouseButton mouseButton, Action action)
        {
            _mouseButtonDictionary.Remove(mouseButton, action);
        }
    }
}