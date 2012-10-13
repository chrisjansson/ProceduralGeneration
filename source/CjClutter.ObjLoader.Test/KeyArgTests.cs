using CjClutter.OpenGl;
using NUnit.Framework;
using OpenTK.Input;
using FluentAssertions;

namespace ObjLoader.Test
{
    [TestFixture]
    public class KeyArgTests
    {
        private KeyDictionary _keys;
        private KeyArg _left;
        private KeyArg _right;

        [SetUp]
        public void SetUp()
        {
            _keys = new KeyDictionary();

            _left = new KeyArg(Key.Left);
            _right = new KeyArg(Key.Right);
        }

        [Test]
        public void IsArgumentTrue_returns_false_when_key_is_not_down()
        {
            var keyArg = new KeyArg(Key.A);

            var isArgumentTrue = keyArg.IsArgumentTrue(_keys);
            isArgumentTrue.Should().Be(false);
        }

        [Test]
        public void IsArgumentTrue_returns_true_when_key_is_down()
        {
            var keyArg = new KeyArg(Key.Left);
            _keys[Key.Left] = true;

            var isArgumentTrue = keyArg.IsArgumentTrue(_keys);
            isArgumentTrue.Should().Be(true);
        }

        [Test]
        public void Combining_two_args_returns_new_arg_with_all_keys()
        {
            var combination = _left && _right;

            combination.Should().NotBeNull();
            combination.Keys.Should().HaveCount(2);
            combination.Keys.Should().Contain(Key.Left);
            combination.Keys.Should().Contain(Key.Right);
        }

        [Test]
        public void Combining_three_args_returns_new_arg_with_all_keys()
        {
            var space = new KeyArg(Key.Space);
            var combination = _left && _right && space;

            combination.Should().NotBeNull();
            combination.Keys.Should().HaveCount(3);
            combination.Keys.Should().Contain(Key.Left);
            combination.Keys.Should().Contain(Key.Right);
            combination.Keys.Should().Contain(Key.Space);
        }

        [Test]
        public void Combining_two_args_returns_false_when_no_keys_are_down()
        {
            var combination = _left && _right;

            var isArgumentTrue = combination.IsArgumentTrue(_keys);
            isArgumentTrue.Should().BeFalse();
        }

        [Test]
        public void Combining_two_args_returns_false_when_first_key_is_down()
        {
            var combination = _left && _right;
            _keys[Key.Left] = true;

            var isArgumentTrue = combination.IsArgumentTrue(_keys);
            isArgumentTrue.Should().BeFalse();
        }

        [Test]
        public void Combining_two_args_returns_false_when_second_key_is_down()
        {
            var combination = _left && _right;
            _keys[Key.Right] = true;

            var isArgumentTrue = combination.IsArgumentTrue(_keys);
            isArgumentTrue.Should().BeFalse();
        }

        [Test]
        public void Combining_two_args_returns_true_when_both_keys_are_down()
        {
            var combination = _left && _right;
            _keys[Key.Left] = true;
            _keys[Key.Right] = true;

            var isArgumentTrue = combination.IsArgumentTrue(_keys);
            isArgumentTrue.Should().BeTrue();
        }
    }
}