using System;
using System.Collections.Generic;
using System.Linq;
using Awesomium.Core;
using CjClutter.OpenGl;
using NUnit.Framework;
using OpenTK.Input;

namespace ObjLoader.Test
{
    [TestFixture]
    public class OpenTKToAwesomiumKeyMapperTests
    {
        [Test, TestCaseSource("KnownKeys")]
        public void Maps_known_key(Key key)
        {
            Assert.AreNotEqual(VirtualKey.UNKNOWN, new OpenTkToAwesomiumKeyMapper().Map(key));
        }

        private IEnumerable<TestCaseData> KnownKeys()
        {
            return Enum.GetValues(typeof(Key))
                .Cast<Key>()
                .Select(x => new TestCaseData(x))
                .ToList();
        }
    }
}