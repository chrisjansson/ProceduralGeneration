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
            var result = _sut.Generate(0, 2,
                                       4, 8, 1);

            result.Should().BeEquivalentTo(0.0, 1.0, 2.0,
                                           2.0, 3.5, 5.0,
                                           4.0, 6.0, 8.0);
        }

        [Test]
        public void Two_sub_divisions_returns_sub_divided_rectangles()
        {
            var result = _sut.Generate(0, 2, 4, 8, 2);

            result.Should().BeEquivalentTo(
                0.0, 0.5, 1.0, 1.5, 2.0,
                1.0, 1.625, 2.25, 2.875, 3.5,
                2.0, 2.75, 3.5, 4.25, 5.0,
                3.0, 3.875, 4.75, 5.625, 6.5,
                4.0, 5.0, 6.0, 7.0, 8.0);
        }

        [Test]
        public void Three_subdivisions_should_have_correct()
        {
            var result = _sut.Generate(0, 2, 4, 8, 3);

            result.Should().HaveCount(81);
        }
    }
}