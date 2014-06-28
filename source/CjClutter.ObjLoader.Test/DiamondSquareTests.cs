using System;
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
            var diamondSquare = new DiamondSquare();

            var result = diamondSquare.Create(0, 1, 2, 3, iterations);

            result.Should().HaveCount(expectedSize);
        }

        [Test]
        public void Has_correct_values_for_zeroth_iteration()
        {
            var diamondSquare = new DiamondSquare();

            var result = diamondSquare.Create(0, 1, 4, 8, 0);

            result.Should().BeEquivalentTo(new double[] 
            {
                0, 1,
                4, 8
            });
        }

        [Test]
        public void Has_correct_values_for_first_iteration()
        {
            var diamondSquare = new DiamondSquare();

            var result = diamondSquare.Create(0, 1, 4, 8, 1);

            result.Should().ContainInOrder(new[]
            {
                0,      1.0625,      1,
                1.8125,   3.25,   3.0625,
                4,      3.8125,      8
            });


        }

        [Test]
        public void Has_correct_values_for_second_iteration()
        {
            var diamondSquare = new DiamondSquare();

            var result = diamondSquare.Create(0, 1, 4, 8, 2);

            result.Should().BeEquivalentTo(new[]
            {
                0,              0.6484375,      1.0625,         1.0390625,      1,
                0.8359375,      1.53125,        1.984375,       2.09375,        1.5390625,
                1.8125,         2.453125,       3.25,           3.234375,       3.0625,
                2.2578125,      3.21875,        3.703125,       4.53125,        3.8984375,
                4,              2.7578125,      3.8125,         4.0859375,      8
            });
        }
    }
}