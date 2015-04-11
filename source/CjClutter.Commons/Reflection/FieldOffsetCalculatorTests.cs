using System.Runtime.InteropServices;
using NUnit.Framework;
using FluentAssertions;

namespace CjClutter.Commons.Reflection
{
    [TestFixture]
    public class FieldOffsetCalculatorTests
    {
        [Test]
        public void Calculates_offset_correctly_for_first_field()
        {
            var offset = FieldOffsetCalculator.CalculateFieldOffset((TestFieldStruct x) => x.FirstField);

            offset.Should().Be(0);
        }

        [Test]
        public void Calculates_offset_correctly_for_second_field()
        {
            var offset = FieldOffsetCalculator.CalculateFieldOffset((TestFieldStruct x) => x.SecondField);

            offset.Should().Be(4);
        }

        [Test]
        public void Calculates_offset_correctly_for_inner_struct_field()
        {
            var offset = FieldOffsetCalculator.CalculateFieldOffset((TestFieldStruct x) => x.InnerStructField);

            offset.Should().Be(12);
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct TestFieldStruct
        {
            public readonly int FirstField;
            public readonly double SecondField;
            public readonly InnerStruct InnerStructField;
        }

        private struct InnerStruct
        {
            public readonly float FirstField;

            public InnerStruct(float firstField)
            {
                FirstField = firstField;
            }
        }
    }
}
