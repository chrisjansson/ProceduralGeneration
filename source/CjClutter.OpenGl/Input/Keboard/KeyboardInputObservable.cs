using System;
using CjClutter.Commons.Collections;
using CjClutter.Commons.Comparators;

namespace CjClutter.OpenGl.Input.Keboard
{
    public class KeyboardInputObservable
    {
        private readonly KeyboardInputProcessor _keyboardInputProcessor;
        private readonly MultiValueDictionary<KeyArg, Action> _keyDictionary;

        public KeyboardInputObservable(KeyboardInputProcessor keyboardInputProcessor)
        {
            _keyboardInputProcessor = keyboardInputProcessor;

            var delegateComparer = new DelegateComparer();
            _keyDictionary = new MultiValueDictionary<KeyArg, Action>(delegateComparer);
        }

        public void SubscribeKey(KeyArg keyArg, Action action)
        {
            _keyDictionary.Add(keyArg, action);
        }

        public void ProcessKeys()
        {
            foreach (var keyArg in _keyDictionary.Keys)
            {
                ProcessKeyArg(keyArg);
            }
        }

        private void ProcessKeyArg(KeyArg keyArg)
        {
            var keyDictionary = _keyboardInputProcessor.KeyDictionary;

            var isArgumentTrue = keyArg.IsArgumentTrue(keyDictionary);
            if(isArgumentTrue)
            {
                FireKeyArgAction(keyArg);
            }
        }

        private void FireKeyArgAction(KeyArg keyArg)
        {
            var actions = _keyDictionary[keyArg];
            foreach (var action in actions)
            {
                action();
            }
        }

        //public void ProcessKeys()
        //{
        //    foreach (var key in _keyDictionary.Keys)
        //    {
        //        ProcessKey(key);
        //    }
        //}

        //private void ProcessKey(Key key)
        //{
        //    if (_actionEvaluator.ShouldKeyActionBeFired(key))
        //    {
        //        FireKeyAction(key);
        //    }
        //}

        //private void FireKeyAction(Key key)
        //{
        //    var mouseButtonActions = _keyDictionary[key];
        //    foreach (var mouseButtonAction in mouseButtonActions)
        //    {
        //        mouseButtonAction();
        //    }
        //}

        //public void SubscribeKey(Key key, Action action)
        //{
        //    _keyDictionary.Add(key, action);
        //}

        //public void UnsubscribeKey(Key key, Action action)
        //{
        //    _keyDictionary.Remove(key, action);
        //}
    }
}
