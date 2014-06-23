using CjClutter.OpenGl.Noise;
using FluentAssertions;
using NUnit.Framework;

namespace ObjLoader.Test
{
    public class FakeHeightOffsetGenerator : IHeightOffsetGenerator
    {
        public double Offset { get; set; }
        public double? SideLength { get; set; }

        public double GetHeightOffset(double sideLength)
        {
            SideLength = sideLength;
            return Offset;
        }
    }

    public class MidpointDisplacementTests
    {
        private MidpointDisplacement _sut;
        private FakeHeightOffsetGenerator _fakeHeightOffsetGenerator;

        [SetUp]
        public void Setup()
        {
            _fakeHeightOffsetGenerator = new FakeHeightOffsetGenerator();
            _sut = new MidpointDisplacement(_fakeHeightOffsetGenerator);
        }

        [Test]
        public void Zero_sub_divisions_returns_original_corners()
        {
            var result = _sut.Generate(0, 2, 4, 8, 0, 0);

            result.Should().BeEquivalentTo(0.0, 2.0, 4.0, 8.0);
        }

        [Test]
        public void One_sub_division_returns_sub_divided_rectangles()
        {
            _fakeHeightOffsetGenerator.Offset = 2;

            var result = _sut.Generate(0, 2,
                                       4, 8, 1, 1024);

            result.Should().BeEquivalentTo(0.0, 1.0, 2.0,
                                           2.0, 5.5, 5.0,
                                           4.0, 6.0, 8.0);
            _fakeHeightOffsetGenerator.SideLength.Should().BeInRange(512.0, 512.0);
        }

        [Test]
        public void Two_sub_divisions_returns_sub_divided_rectangles()
        {
            var result = _sut.Generate(0, 2, 4, 8, 2, 0);

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
            var result = _sut.Generate(0, 2, 4, 8, 3, 0);

            result.Should().HaveCount(81);
        }
    }
}