using NUnit.Framework;

namespace CjClutter.Commons.Comparators
{
    [TestFixture]
    public class ByteArrayComparerTests
    {
        private ByteArrayComparer _comparer;

        [SetUp]
        public void SetUp()
        {
            _comparer = new ByteArrayComparer();
        }

        [Test]
        public void Equals_two_empty_arrays_returns_true()
        {
            var a = new byte[] { };
            var b = new byte[] { };

            Assert.IsTrue(_comparer.Equals(a, b));
        }

        [Test]
        public void Equals_two_arrays_of_different_length_returns_false()
        {
            var a = new byte[] { 1 };
            var b = new byte[] { 1, 2 };

            Assert.IsFalse(_comparer.Equals(a, b));
        }

        [Test]
        public void Equals_two_arrays_of_same_length_but_different_contents_returns_false()
        {
            var a = new byte[] { 1, 3 };
            var b = new byte[] { 1, 2 };

            Assert.IsFalse(_comparer.Equals(a, b));
        }
    }
}
