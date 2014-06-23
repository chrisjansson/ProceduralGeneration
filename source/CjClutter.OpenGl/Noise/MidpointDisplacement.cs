using System;

namespace CjClutter.OpenGl.Noise
{
    public interface IHeightOffsetGenerator
    {
        double GetHeightOffset(double sideLength);
    }

    public class RandomHeightOffsetGenerator : IHeightOffsetGenerator
    {
        private readonly Random _random;
        private readonly double _roughness;

        public RandomHeightOffsetGenerator(int seed, double roughness)
        {
            _roughness = roughness;
            _random = new Random(seed);
        }

        public double GetHeightOffset(double sideLength)
        {
            var r = (_random.NextDouble() - 0.5)*2;
            return r*sideLength*_roughness;
        }
    }

    public class MidpointDisplacement
    {
        private readonly IHeightOffsetGenerator _heightOffsetGenerator;

        public MidpointDisplacement(IHeightOffsetGenerator heightOffsetGenerator)
        {
            _heightOffsetGenerator = heightOffsetGenerator;
        }

        public double[] Generate(double h0, double h1, double h2, double h3, int levels, double quadLength)
        {
            var old = new[] { h0, h1, h2, h3 };
            var result = old;

            var rowVertices = 2; //2^levels + 1
            var tempSideLength = quadLength;
            for (var i = 0; i < levels; i++)
            {
                tempSideLength /= 2;
                result = Subdivide(old, rowVertices, tempSideLength);
                old = result;
                rowVertices = (rowVertices - 1)*2 + 1;
            }

            return result;
        }

        private double[] Subdivide(double[] old, int rowVertices, double sideLength)
        {
            var newRowVertices = (rowVertices - 1) * 2 + 1;
            var newValues = new double[newRowVertices * newRowVertices];

            for (var oldRow = 0; oldRow < rowVertices; oldRow++)
            {
                for (int oldColumn = 0; oldColumn < rowVertices; oldColumn++)
                {
                    var newIndex = oldRow * newRowVertices * 2 + oldColumn * 2;
                    newValues[newIndex] = old[rowVertices * oldRow + oldColumn];
                }
            }

            for (int row = 0; row < newRowVertices; row += 2)
            {
                for (int column = 1; column < newRowVertices; column += 2)
                {
                    var index = row * newRowVertices + column;
                    var value = (newValues[index - 1] + newValues[index + 1]) / 2;
                    newValues[index] = value;
                }
            }

            for (int row = 1; row < newRowVertices; row += 2)
            {
                for (int column = 0; column < newRowVertices; column += 2)
                {
                    var index = row * newRowVertices + column;
                    var previous = (row - 1) * newRowVertices + column;
                    var next = (row + 1) * newRowVertices + column;

                    var value = (newValues[previous] + newValues[next]) / 2;
                    newValues[index] = value;
                }
            }

            for (var row = 1; row < newRowVertices; row += 2)
            {
                for (var column = 1; column < newRowVertices; column += 2)
                {
                    var index = row * newRowVertices + column;
                    var previous = (row - 1) * newRowVertices + column;
                    var next = (row + 1) * newRowVertices + column;

                    var value = (newValues[previous] + newValues[next] + newValues[index - 1] + newValues[index + 1]) / 4;
                    var offset = _heightOffsetGenerator.GetHeightOffset(sideLength);
                    newValues[index] = value + offset;
                }
            }

            return newValues;
        }
    }
}