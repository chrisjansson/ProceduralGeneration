using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Input;

namespace CjClutter.OpenGl.Input
{
    public class MouseInputObservable
    {
        private readonly MouseInputProcessor _mouseInputProcessor;
        private readonly MultiValueDictionary<MouseButton, Action> _mouseButtonDictionary;

        public MouseInputObservable(MouseInputProcessor mouseInputProcessor)
        {
            _mouseInputProcessor = mouseInputProcessor;

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
            if (_mouseInputProcessor.WasButtonPressed(mouseButton))
            {
                FireMouseDown(mouseButton);
            }
            else if (_mouseInputProcessor.WasButtonReleased(mouseButton))
            {
                FireMouseUp(mouseButton);
            }
        }

        private void FireMouseDown(MouseButton mouseButton)
        {
            var mouseButtonActions = _mouseButtonDictionary[mouseButton];
            foreach (var mouseButtonAction in mouseButtonActions)
            {
                mouseButtonAction();
            }
        }

        private void FireMouseUp(MouseButton mouseButton)
        {
            var mouseButtonActions = _mouseButtonDictionary[mouseButton];
            foreach (var mouseButtonAction in mouseButtonActions)
            {
                mouseButtonAction();
            }
        }

        public void Subscribe(MouseButton mouseButton, Action action)
        {
            _mouseButtonDictionary.Add(mouseButton, action);
        }

        public void Unsubscribe(MouseButton mouseButton, Action action)
        {
            _mouseButtonDictionary.Remove(mouseButton, action);
        }
    }
}