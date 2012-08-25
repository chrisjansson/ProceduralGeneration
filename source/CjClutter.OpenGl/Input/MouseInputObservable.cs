using System;
using OpenTK.Input;

namespace CjClutter.OpenGl.Input
{
    public class MouseInputObservable
    {
        private readonly MouseInputProcessor _mouseInputProcessor;

        private readonly MultiValueDictionary<MouseButton, Action> _mouseButtonDownDictionary;
        private MultiValueDictionary<MouseButton, Action> _mouseButtonUpDictionary;
        private MultiValueDictionary<MouseButton, Action> _mouseButtonChangedDictionary;

        public MouseInputObservable(MouseInputProcessor mouseInputProcessor)
        {
            _mouseInputProcessor = mouseInputProcessor;

            var delegateComparer = new DelegateComparer();

            _mouseButtonDownDictionary = new MultiValueDictionary<MouseButton, Action>(delegateComparer);
            _mouseButtonUpDictionary = new MultiValueDictionary<MouseButton, Action>(delegateComparer);
            _mouseButtonChangedDictionary = new MultiValueDictionary<MouseButton, Action>(delegateComparer);
        }

        public void ProcessMouseButtons()
        {
            foreach (var mouseButton in _mouseButtonDownDictionary.Keys)
            {
                ProcessMouseButton(mouseButton);
            }
        }

        private void ProcessMouseButton(MouseButton mouseButton)
        {
            if (_mouseInputProcessor.WasButtonPressed(mouseButton))
            {
                FireMouseDown(mouseButton);
                FireMouseChanged(mouseButton);

            }
            else if (_mouseInputProcessor.WasButtonReleased(mouseButton))
            {
                FireMouseUp(mouseButton);
                FireMouseChanged(mouseButton);
            }
        }

        private void FireMouseChanged(MouseButton mouseButton)
        {
            
        }

        private void FireMouseDown(MouseButton mouseButton)
        {
            var mouseButtonActions = _mouseButtonDownDictionary[mouseButton];
            foreach (var mouseButtonAction in mouseButtonActions)
            {
                mouseButtonAction();
            }
        }

        private void FireMouseUp(MouseButton mouseButton)
        {
            //var mouseButtonActions = _mouseButtonDownDictionary[mouseButton];
            //foreach (var mouseButtonAction in mouseButtonActions)
            //{
            //    mouseButtonAction();
            //}
        }

        public void SubscribeMouseButtonDown(MouseButton mouseButton, Action action)
        {
            _mouseButtonDownDictionary.Add(mouseButton, action);
        }

        public void UnsubscribeMouseButtonDown(MouseButton mouseButton, Action action)
        {
            _mouseButtonDownDictionary.Remove(mouseButton, action);
        }

        public void SubscribeMouseButtonUp(MouseButton mouseButton, Action action)
        {
            //_mouseButtonDownDictionary.Add(mouseButton, action);
        }

        public void UnsubscribeMouseButtonUp(MouseButton mouseButton, Action action)
        {
            //_mouseButtonDownDictionary.Remove(mouseButton, action);
        }
    }
}