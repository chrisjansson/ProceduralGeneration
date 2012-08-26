using System;
using OpenTK.Input;

namespace CjClutter.OpenGl.Input.Keboard
{
    public class KeyboardInputObservable
    {
        private readonly IKeyActionEvaluator _actionEvaluator;
        private readonly MultiValueDictionary<Key, Action> _keyDictionary;

        public KeyboardInputObservable(IKeyActionEvaluator actionEvaluator)
        {
            _actionEvaluator = actionEvaluator;

            var delegateComparer = new DelegateComparer();
            _keyDictionary = new MultiValueDictionary<Key, Action>(delegateComparer);
        }

        public void ProcessKeys()
        {
            foreach (var key in _keyDictionary.Keys)
            {
                ProcessKey(key);
            }
        }

        private void ProcessKey(Key key)
        {
            if (_actionEvaluator.ShouldKeyActionBeFired(key))
            {
                FireKeyAction(key);
            }
        }

        private void FireKeyAction(Key key)
        {
            var mouseButtonActions = _keyDictionary[key];
            foreach (var mouseButtonAction in mouseButtonActions)
            {
                mouseButtonAction();
            }
        }

        public void SubscribeKey(Key key, Action action)
        {
            _keyDictionary.Add(key, action);
        }

        public void UnsubscribeKey(Key key, Action action)
        {
            _keyDictionary.Remove(key, action);
        }
    }
}
