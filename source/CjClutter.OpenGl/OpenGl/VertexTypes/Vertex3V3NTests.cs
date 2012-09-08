using CjClutter.Commons.Reflection;
using NUnit.Framework;
using FluentAssertions;

namespace CjClutter.OpenGl.OpenGl.VertexTypes
{
    [TestFixture]
    public class Vertex3V3NTests
    {
        private Vertex3V3N _vertex3V3N;

        [SetUp]
        public void SetUp()
        {
            _vertex3V3N = new Vertex3V3N();
        }

        [Test]
        public void Has_correct_size()
        {
            var actualSize = TypeSizeCalculator.GetSize<Vertex3V3N>();

            _vertex3V3N.SizeInBytes.Should().Be(actualSize);
        }

        [Test]
        public void Fields_has_correct_offsets()
        {
            var expectedPositionOffset = FieldOffsetCalculator.CalculateFieldOffset((Vertex3V3N x) => x.Position);
            var expectedNormalOffset = FieldOffsetCalculator.CalculateFieldOffset((Vertex3V3N x) => x.Normal);

            _vertex3V3N.PositionOffset.Should().Be(expectedPositionOffset);
            _vertex3V3N.NormalOffset.Should().Be(expectedNormalOffset);
        }
    }
}
