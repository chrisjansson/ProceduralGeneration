using System;
using CjClutter.OpenGl;
using NUnit.Framework;
using FluentAssertions;
using OpenTK.Input;
using System.Linq;

namespace ObjLoader.Test
{
    [TestFixture]
    public class KeyDictionaryTests
    {
        [Test]
        public void Can_instantiate_the_dictionary()
        {
            Action act = () => new KeyDictionary();

            act.ShouldNotThrow();
        }

        [Test]
        public void Has_all_keys()
        {
            var keys = typeof (Key)
                .GetEnumValues()
                .Cast<Key>()
                .ToList();

            var keyDictionary = new KeyDictionary();

            foreach (var key in keys)
            {
                var key1 = key;
                Action act = () => { var b = keyDictionary[key1]; };

                act.ShouldNotThrow();
            }
        }


    }
}