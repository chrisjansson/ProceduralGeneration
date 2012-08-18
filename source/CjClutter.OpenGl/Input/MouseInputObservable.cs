using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Input;

namespace CjClutter.OpenGl.Input
{
    public class MouseInputObservable
    {
        private readonly MouseInputProcessor _mouseInputProcessor;
        private readonly Dictionary<MouseButton, List<Action>> _mouseButtonDictionary;

        public MouseInputObservable(MouseInputProcessor mouseInputProcessor)
        {
            _mouseInputProcessor = mouseInputProcessor;
            _mouseButtonDictionary = new Dictionary<MouseButton, List<Action>>();
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
            List<Action> actions;
            var hasActions = _mouseButtonDictionary.TryGetValue(mouseButton, out actions);

            if (!hasActions)
            {
                actions = new List<Action>();
                _mouseButtonDictionary.Add(mouseButton, actions);
            }

            actions.Add(action);
        }

        public void Unsubscribe(MouseButton mouseButton, Action action)
        {
            List<Action> actions;
            var hasActions = _mouseButtonDictionary.TryGetValue(mouseButton, out actions);

            if (!hasActions)
            {
                return;
            }

            var actionsToRemove = actions.Where(x => x.Target == action.Target && x.Method == action.Method);
            foreach (var actionToRemove in actionsToRemove)
            {
                actions.Remove(actionToRemove);
            }
        }
    }
}