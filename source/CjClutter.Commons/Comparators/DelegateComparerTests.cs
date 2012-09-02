using System;
using NUnit.Framework;

namespace CjClutter.Commons.Comparators
{
    [TestFixture]
    public class DelegateComparerTests
    {
        private DelegateComparer _comparer;

        [SetUp]
        public void SetUp()
        {
            _comparer = new DelegateComparer();
        }

        [Test]
        public void Equals_equal_lambda_actions_returns_true()
        {
            var value = 0;
            Action lambda1 = () => value = 3;
            Action lambda2 = () => value = 3;

            Assert.IsTrue(_comparer.Equals(lambda1, lambda2));
        }

        [Test]
        public void Equals_unequal_lambda_actions_returns_false()
        {
            var value = 0;
            Action lambda1 = () => value = 3;
            Action lambda2 = () => value = 5;

            Assert.IsFalse(_comparer.Equals(lambda1, lambda2));
        }

        [Test]
        public void Equals_equal_method_actions_returns_true()
        {
            var action1 = new Action(FirstMethod);
            var action2 = new Action(FirstMethod);

            Assert.IsTrue(_comparer.Equals(action1, action2));
        }

        [Test]
        public void Equals_not_equal_method_actions_returns_false()
        {
            var action1 = new Action(FirstMethod);
            var action2 = new Action(SecondMethod);

            Assert.IsFalse(_comparer.Equals(action1, action2));
        }

        private void FirstMethod()
        {
            var a = 3;
        }

        private void SecondMethod()
        {
            var b = 4;
        }
    }
}
