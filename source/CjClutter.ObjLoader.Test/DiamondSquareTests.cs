using System;
using System.Collections.Generic;
using CjClutter.OpenGl;
using FluentAssertions;
using NUnit.Framework;

namespace ObjLoader.Test
{
    public class DiamondSquareTests
    {
        [TestCase(0, 4)]
        [TestCase(1, 9)]
        [TestCase(2, 25)]
        [TestCase(3, 81)]
        public void Correct_size_depending_on_number_of_iterations(int iterations, int expectedSize)
        {
            var diamondSquare = new DiamondSquare(new NullOffsetGenerator());

            var result = diamondSquare.Create(0, 1, 2, 3, iterations, 0);

            result.Should().HaveCount(expectedSize);
        }

        [Test]
        public void Has_correct_values_for_zeroth_iteration()
        {
            var diamondSquare = new DiamondSquare(new NullOffsetGenerator());

            var result = diamondSquare.Create(0, 1, 4, 8, 0, 0);

            result.Should().BeEquivalentTo(new double[] 
            {
                0, 1,
                4, 8
            });
        }

        [Test]
        public void Has_correct_values_for_first_iteration()
        {
            var diamondSquare = new DiamondSquare(new NullOffsetGenerator());

            var result = diamondSquare.Create(0, 1, 4, 8, 1, 0);

            result.Should().ContainInOrder(new[]
            {
                0,      1.0625,      1,
                1.8125,   3.25,   3.0625,
                4,      3.8125,      8
            });


        }

        [Test]
        public void Offsets_heights()
        {
            var diamondSquare = new DiamondSquare(new FakeOffsetGenerator(
                new FakeOffsetGenerator.FakeOffset(1, 1, 128, 0.25),
                new FakeOffsetGenerator.FakeOffset(1, -1, 128, 1),
                new FakeOffsetGenerator.FakeOffset(-1, 1, 128, 2),
                new FakeOffsetGenerator.FakeOffset(3, 1, 128, -1),
                new FakeOffsetGenerator.FakeOffset(1, 3, 128, 1),
                new FakeOffsetGenerator.FakeOffset(1, 0, 128, 1),
                new FakeOffsetGenerator.FakeOffset(1, 2, 128, 2),
                new FakeOffsetGenerator.FakeOffset(0, 1, 128, 1),
                new FakeOffsetGenerator.FakeOffset(2, 1, 128, 1)));

            var result = diamondSquare.Create(0, 1, 4, 8, 1, 128);

            result.Should().ContainInOrder(new[]
            {
                0,      1.625,  1,
                2.625,  3.5,    3.125,
                4,      4.325,  8
            });
        }

        [Test]
        public void Has_correct_values_for_second_iteration()
        {
            var diamondSquare = new DiamondSquare(new NullOffsetGenerator());

            var result = diamondSquare.Create(0, 1, 4, 8, 2, 0);

            result.Should().BeEquivalentTo(new[]
            {
                0,              0.6484375,      1.0625,         1.0390625,      1,
                0.8359375,      1.53125,        1.984375,       2.09375,        1.5390625,
                1.8125,         2.453125,       3.25,           3.234375,       3.0625,
                2.2578125,      3.21875,        3.703125,       4.53125,        3.8984375,
                4,              2.7578125,      3.8125,         4.0859375,      8
            });
        }

        private class NullOffsetGenerator : IDiamondSquareHeightOffsetGenerator
        {
            public double Get(int x, int y, double sideLength)
            {
                return 0;
            }
        }

        private class FakeOffsetGenerator : IDiamondSquareHeightOffsetGenerator
        {
            private Dictionary<Key, double> _values;

            public struct FakeOffset
            {
                public int X { get; set; }
                public int Y { get; set; }
                public double SideLength { get; set; }
                public double Offset { get; set; }

                public FakeOffset(int x, int y, double sideLength, double offset)
                    : this()
                {
                    X = x;
                    Y = y;
                    SideLength = sideLength;
                    Offset = offset;
                }
            }

            private struct Key
            {
                public int X { get; set; }
                public int Y { get; set; }
                public double SideLength { get; set; }
            }

            public FakeOffsetGenerator(params FakeOffset[] offsets)
            {
                _values = new Dictionary<Key, double>();

                foreach (var offset in offsets)
                {
                    _values.Add(new Key { X = offset.X, Y = offset.Y, SideLength = offset.SideLength }, offset.Offset);
                }
            }

            public double Get(int x, int y, double sideLength)
            {
                return _values[new Key { X = x, Y = y, SideLength = sideLength }];
            }
        }
    }
}