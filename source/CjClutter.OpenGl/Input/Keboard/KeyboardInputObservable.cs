using System;
using System.Collections.Generic;
using System.Linq;

namespace CjClutter.OpenGl.Input.Keboard
{
    public class KeyboardInputObservable
    {
        private readonly KeyboardInputProcessor _keyboardInputProcessor;

        private readonly HashSet<KeyCombination> _keyArgsDown = new HashSet<KeyCombination>();

        private readonly List<KeyArgActionPair> _keyArgDownPairs = new List<KeyArgActionPair>();
        private readonly List<KeyArgActionPair> _keyArgUpPairs = new List<KeyArgActionPair>();

        private ILookup<KeyCombination, KeyArgActionPair> _keyArgUpLookUp;
        private ILookup<KeyCombination, KeyArgActionPair> _keyArgDownLookUp;
        private IEnumerable<KeyCombination> _allKeyArgs;

        public KeyboardInputObservable(KeyboardInputProcessor keyboardInputProcessor)
        {
            _keyboardInputProcessor = keyboardInputProcessor;
        }

        public void SubscribeKey(KeyCombination keyCombination, CombinationDirection keyargDirection, Action action)
        {
            var keyArgActionPair = new KeyArgActionPair { KeyCombination = keyCombination, Action = action };

            if ((keyargDirection & CombinationDirection.Up) == CombinationDirection.Up)
            {
                _keyArgUpPairs.Add(keyArgActionPair);
            }

            if ((keyargDirection & CombinationDirection.Down) == CombinationDirection.Down)
            {
                _keyArgDownPairs.Add(keyArgActionPair);
            }

            GenerateLookups();
        }

        private void GenerateLookups()
        {
            _keyArgUpLookUp = _keyArgUpPairs.ToLookup(x => x.KeyCombination);
            _keyArgDownLookUp = _keyArgDownPairs.ToLookup(x => x.KeyCombination);


            var downKeyArgs = _keyArgDownPairs.Select(x => x.KeyCombination);
            var upKeyArgs = _keyArgUpPairs.Select(x => x.KeyCombination);

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

        private void ProcessKeyArg(KeyCombination keyCombination)
        {
            var keyDictionary = _keyboardInputProcessor.KeyDictionary;

            var isArgumentTrue = keyCombination.IsArgumentTrue(keyDictionary);
            var keyArgIsDown = _keyArgsDown.Contains(keyCombination);

            if (isArgumentTrue && !keyArgIsDown)
            {
                KeyArgDown(keyCombination);
            }
            else if (!isArgumentTrue && keyArgIsDown)
            {
                KeyArgUp(keyCombination);
            }
        }

        private void KeyArgUp(KeyCombination keyCombination)
        {
            _keyArgsDown.Remove(keyCombination);
            FireKeyArgUp(keyCombination);
        }

        private void KeyArgDown(KeyCombination keyCombination)
        {
            _keyArgsDown.Add(keyCombination);
            FireKeyArgDown(keyCombination);
        }

        private void FireKeyArgUp(KeyCombination keyCombination)
        {
            var actions = _keyArgUpLookUp[keyCombination];
            FireKeyActionPairActions(actions);
        }

        private void FireKeyArgDown(KeyCombination keyCombination)
        {
            var actions = _keyArgDownLookUp[keyCombination];
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