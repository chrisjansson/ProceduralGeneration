using NUnit.Framework;
using FluentAssertions;

namespace CjClutter.OpenGl.OpenGl.VertexTypes
{
    [TestFixture]
    public class VertexTests
    {
        private Vertex3V _vertex3V;

        [SetUp]
        public void SetUp()
        {
            _vertex3V = new Vertex3V();
        }

        [Test]
        public void Has_correct_size()
        {
            var actualSize = TypeSizeCalculator.GetSize<Vertex3V>();

            _vertex3V.SizeInBytes.Should().Be(actualSize);
        }
    }
}
