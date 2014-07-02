using System;
using CjClutter.OpenGl.Noise;

namespace CjClutter.OpenGl
{
    public interface IDiamondSquareHeightOffsetGenerator
    {
        double Get(int x, int y, double sideLength);
    }

    public class DiamondSquareHeightOffsetGenerator : IDiamondSquareHeightOffsetGenerator
    {
        private readonly ImprovedPerlinNoise _improvedPerlinNoise;
        private readonly Random _random;
        private readonly double _roughness;

        public DiamondSquareHeightOffsetGenerator(int seed, double roughness)
        {
            _roughness = roughness;
            _improvedPerlinNoise = new ImprovedPerlinNoise(seed);
            _random = new Random(seed);
        }

        public double Get(int x, int y, double sideLength)
        {
            if (x <= 0 || x >= 256 || y <= 0 || y >= 256)
                return _improvedPerlinNoise.Noise(x / 256.0, y / 256.0);

            return (_random.NextDouble() - 0.5) * 2 * sideLength * _roughness;
        }
    }

    public class DiamondSquare
    {
        private double[] _values;
        private int _sideLength;
        private double _length;
        private readonly IDiamondSquareHeightOffsetGenerator _heightOffsetGenerator;

        public DiamondSquare(IDiamondSquareHeightOffsetGenerator heightOffsetGenerator)
        {
            _heightOffsetGenerator = heightOffsetGenerator;
        }

        public double[] Create(double p0, double p1, double p2, double p3, int iterations, double sideLength)
        {
            _sideLength = (int)Math.Pow(2, iterations) + 1;
            var size = _sideLength * _sideLength;

            _values = new double[size];

            _values[0] = p0;
            _values[_sideLength - 1] = p1;
            _values[size - _sideLength] = p2;
            _values[size - 1] = p3;

            _length = sideLength;
            for (var distance = (_sideLength - 1) / 2; distance >= 1; distance /= 2)
            {
                for (var x = distance; x < _sideLength - 1; x += distance * 2)
                {
                    for (var y = distance; y < _sideLength - 1; y += distance * 2)
                    {
                        Diamond(x, y, distance);
                    }

                }

                //Column square corners
                for (var x = distance; x < _sideLength; x += distance * 2)
                {
                    for (var y = 0; y < _sideLength; y += distance * 2)
                    {
                        Square(x, y, distance);
                    }
                }

                //Row square corners
                for (var x = 0; x < _sideLength; x += distance * 2)
                {
                    for (var y = distance; y < _sideLength; y += distance * 2)
                    {
                        Square(x, y, distance);
                    }
                }

                _length /= 2;
            }


            return _values;
        }

        private void Square(int x, int y, int distance)
        {
            Set(x, y, (Get(x - distance, y) + Get(x + distance, y) + Get(x, y + distance) + Get(x, y - distance)) / 4);
        }

        private void Diamond(int x, int y, int distance)
        {
            var average = (Get(x + distance, y + distance) + Get(x + distance, y - distance) + Get(x - distance, y + distance) + Get(x - distance, y - distance)) / 4;
            Set(x, y, average);
        }

        private double Get(int x, int y)
        {
            if (x < 0 || x >= _sideLength || y < 0 || y >= _sideLength)
                return _heightOffsetGenerator.Get(x, y, _length);

            return _values[y * _sideLength + x];
        }

        private void Set(int x, int y, double value)
        {
            _values[y * _sideLength + x] = value + _heightOffsetGenerator.Get(x, y, _length);
        }
    }
}