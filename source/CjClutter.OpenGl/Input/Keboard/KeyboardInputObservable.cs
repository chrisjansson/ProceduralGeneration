using System;
using System.Collections.Generic;
using System.Linq;

namespace CjClutter.OpenGl.Input.Keboard
{
    public class KeyboardInputObservable
    {
        private readonly KeyboardInputProcessor _keyboardInputProcessor;

        private readonly HashSet<KeyArg> _keyArgsDown = new HashSet<KeyArg>();

        private readonly List<KeyArgActionPair> _keyArgDownPairs = new List<KeyArgActionPair>();
        private readonly List<KeyArgActionPair> _keyArgUpPairs = new List<KeyArgActionPair>();

        private ILookup<KeyArg, KeyArgActionPair> _keyArgUpLookUp;
        private ILookup<KeyArg, KeyArgActionPair> _keyArgDownLookUp;
        private IEnumerable<KeyArg> _allKeyArgs;

        public KeyboardInputObservable(KeyboardInputProcessor keyboardInputProcessor)
        {
            _keyboardInputProcessor = keyboardInputProcessor;
        }

        public void SubscribeKey(KeyArg keyArg, KeyArgDirection keyargDirection, Action action)
        {
            var keyArgActionPair = new KeyArgActionPair { KeyArg = keyArg, Action = action };

            if ((keyargDirection & KeyArgDirection.Up) == KeyArgDirection.Up)
            {
                _keyArgUpPairs.Add(keyArgActionPair);
            }

            if ((keyargDirection & KeyArgDirection.Down) == KeyArgDirection.Down)
            {
                _keyArgDownPairs.Add(keyArgActionPair);
            }

            GenerateLookups();
        }

        private void GenerateLookups()
        {
            _keyArgUpLookUp = _keyArgUpPairs.ToLookup(x => x.KeyArg);
            _keyArgDownLookUp = _keyArgDownPairs.ToLookup(x => x.KeyArg);


            var downKeyArgs = _keyArgDownPairs.Select(x => x.KeyArg);
            var upKeyArgs = _keyArgUpPairs.Select(x => x.KeyArg);

            _allKeyArgs = downKeyArgs
                .Union(upKeyArgs)
                .Distinct();
        }

        public void ProcessKeys()
        {
            foreach (var keyArg in _allKeyArgs)
            {
                ProcessKeyArg(keyArg);
            }
        }

        private void ProcessKeyArg(KeyArg keyArg)
        {
            var keyDictionary = _keyboardInputProcessor.KeyDictionary;

            var isArgumentTrue = keyArg.IsArgumentTrue(keyDictionary);
            var keyArgIsDown = _keyArgsDown.Contains(keyArg);

            if (isArgumentTrue && !keyArgIsDown)
            {
                KeyArgDown(keyArg);
            }
            else if (!isArgumentTrue && keyArgIsDown)
            {
                KeyArgUp(keyArg);
            }
        }

        private void KeyArgUp(KeyArg keyArg)
        {
            _keyArgsDown.Remove(keyArg);
            FireKeyArgUp(keyArg);
        }

        private void KeyArgDown(KeyArg keyArg)
        {
            _keyArgsDown.Add(keyArg);
            FireKeyArgDown(keyArg);
        }

        private void FireKeyArgUp(KeyArg keyArg)
        {
            var actions = _keyArgUpLookUp[keyArg];
            FireKeyActionPairActions(actions);
        }

        private void FireKeyArgDown(KeyArg keyArg)
        {
            var actions = _keyArgDownLookUp[keyArg];
            FireKeyActionPairActions(actions);
        }

        private void FireKeyActionPairActions(IEnumerable<KeyArgActionPair> keyArgActionPairs)
        {
            foreach (var keyArgActionPair in keyArgActionPairs)
            {
                keyArgActionPair.Action();
            }
        }
    }
}