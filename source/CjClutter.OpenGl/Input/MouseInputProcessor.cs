using System;
using System.Collections.Generic;
using OpenTK.Input;
using System.Linq;

namespace CjClutter.OpenGl.Input
{
    public class MouseInputProcessor
    {
        private MouseState _previousFrameMouseState;
        private MouseState _currentFrameMouseState;
        private readonly Dictionary<MouseButton, List<Action>> _mouseButtonDictionary;


        public MouseInputProcessor()
        {
            _previousFrameMouseState = new MouseState();
            _mouseButtonDictionary = new Dictionary<MouseButton, List<Action>>();
        }

        public void Update(MouseState mouseState)
        {
            _currentFrameMouseState = mouseState;

            foreach (var mouseButton in _mouseButtonDictionary.Keys)
            {
                ProcessMouseButton(mouseButton);
            }

            _previousFrameMouseState = _currentFrameMouseState;
        }

        private void ProcessMouseButton(MouseButton mouseButton)
        {
            var isButtonPressedInPreviousUpdate = _previousFrameMouseState[mouseButton];
            var isButtonPressedInCurrentUpdate = _currentFrameMouseState[mouseButton];

            var buttonStateChanged = isButtonPressedInPreviousUpdate != isButtonPressedInCurrentUpdate;

            if (buttonStateChanged && isButtonPressedInCurrentUpdate)
            {
                FireMouseDown(mouseButton);
            }
            else if (buttonStateChanged)
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
            
            if(!hasActions)
            {
                actions = new List<Action>();
                _mouseButtonDictionary.Add(mouseButton, actions);
            }

            actions.Add(action);
        }

        public void UnSubscribe(MouseButton mouseButton, Action action)
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
