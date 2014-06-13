using CjClutter.OpenGl.Noise;
using FluentAssertions;
using NUnit.Framework;

namespace ObjLoader.Test
{
    public class DiamondSquareTests
    {
        private DiamondSquare _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new DiamondSquare();
        }

        [Test]
        public void Zero_sub_divisions_returns_original_corners()
        {
            var result = _sut.Generate(0, 2, 4, 8, 0);

            result.Should().BeEquivalentTo(0.0, 2.0, 4.0, 8.0);
        }

        [Test]
        public void One_sub_division_returns_sub_divided_rectangles()
        {
            var result = _sut.Generate(0, 2, 4, 8, 1);

            result.Should().BeEquivalentTo(0.0, 1.0, 2.0, 2.0, 3.5, 5.0, 4.0, 6.0, 8.0);
        }
    }
}