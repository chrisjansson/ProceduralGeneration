using System;
using System.Collections.Generic;
using OpenTK.Input;

namespace CjClutter.OpenGl.Input
{
    public class MouseButtonActionDictionary
    {
        private readonly Dictionary<MouseButton, List<Action>> _mouseButtonDictionary;
 
        public void Add(MouseButton mouseButton, Action action)
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


    }
}