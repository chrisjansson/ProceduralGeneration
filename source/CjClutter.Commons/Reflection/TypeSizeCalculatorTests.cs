using NUnit.Framework;
using FluentAssertions;

namespace CjClutter.Commons.Reflection
{
    [TestFixture]
    public class TypeSizeCalculatorTests
    {
        [Test]
        public void GetSize_returns_correct_size_for_float()
        {
            var actualSize = TypeSizeCalculator.GetSize<float>();

            actualSize.Should().Be(4);
        }

        [Test]
        public void GetSize_returns_correct_size_for_double()
        {
            var actualSize = TypeSizeCalculator.GetSize<double>();

            actualSize.Should().Be(8);
        }

        [Test]
        public void GetSize_returns_correct_size_for_byte()
        {
            var actualSize = TypeSizeCalculator.GetSize<byte>();

            actualSize.Should().Be(1);
        }

        [Test]
        public void GetSize_returns_correct_size_for_struct_with_one_member()
        {
            var actualSize = TypeSizeCalculator.GetSize<TestStructWithOneMember>();

            actualSize.Should().Be(4);
        }

        private struct TestStructWithOneMember
        {
            public int IntField { get; set; }
        }
    }
}
