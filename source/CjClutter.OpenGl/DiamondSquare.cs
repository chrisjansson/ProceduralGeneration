using System;

namespace CjClutter.OpenGl
{
    public class DiamondSquare
    {
        private double[] _values;
        private int _sideLength;

        public double[] Create(double p0, double p1, double p2, double p3, int iterations)
        {
            _sideLength = (int)Math.Pow(2, iterations) + 1;
            var size = _sideLength * _sideLength;

            _values = new double[size];

            _values[0] = p0;
            _values[_sideLength - 1] = p1;
            _values[size - _sideLength] = p2;
            _values[size - 1] = p3;

            var left = 0;
            var top = 0;
            var right = _sideLength - 1;
            var bottom = _sideLength - 1;

            for (var distance = (_sideLength - 1) / 2; distance >= 1; distance /= 2)
            {
                Diamond(left, top, right, bottom);

                Square(left, bottom - distance, distance);
                Square(right, bottom - distance, distance);
                Square(right - distance, top, distance);
                Square(right - distance, bottom, distance);
            }

            return _values;
        }

        private void Square(int x, int y, int distance)
        {
            Set(x, y, (Get(x - distance, y) + Get(x + distance, y) + Get(x, y + distance) + Get(x, y - distance)) / 4);
        }

        private void Diamond(int left, int top, int right, int bottom)
        {
            var x = (right - left) / 2;
            var y = (bottom - top) / 2;

            var average = (Get(left, top) + Get(right, top) + Get(left, bottom) + Get(right, bottom)) / 4;
            Set(x, y, average);
        }

        private double Get(int x, int y)
        {
            if (x < 0 || x >= _sideLength || y < 0 || y >= _sideLength)
                return 0;

            return _values[y * _sideLength + x];
        }

        private void Set(int x, int y, double value)
        {
            _values[y * _sideLength + x] = value;
        }
    }
}