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

            if (iterations != 0)
            {
                Diamond(left, top, right, bottom);

                var distance = (right - left)/2;

                Set(left, bottom - distance, (Get(left, top) + Get(left, bottom) + Get(left + distance, top + distance) + Get(left - distance, top + distance)) / 4);
            }

            return _values;
        }

        private void Diamond(int left, int top, int right, int bottom)
        {
            var x = (right - left) / 2;
            var y = (bottom - top) / 2;

            var average = (Get(left, top) + Get(right, top) + Get(left, bottom) + Get(right, bottom))/4;
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
            _values[y*_sideLength + x] = value;
        }
    }
}