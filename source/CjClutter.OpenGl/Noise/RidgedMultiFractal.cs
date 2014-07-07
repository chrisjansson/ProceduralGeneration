using System;
using OpenTK;

namespace CjClutter.OpenGl.Noise
{
/* Ridged multifractal terrain model.
*
* Copyright 1994 F. Kenton Musgrave 
*
* Some good parameter values to start with:
*
*      H:           1.0
*      offset:      1.0
*      gain:        2.0
*/
    public class RidgedMultiFractal : INoiseGenerator
    {
        private readonly INoiseGenerator _baseNoise;

        private readonly double[] _exponentArray;
        private readonly int _octaves;
        private readonly double _lacunarity;
        private readonly double _offset;
        private readonly double _gain;

        public RidgedMultiFractal(INoiseGenerator baseBaseNoise, int octaves, double lacunarity, double H = 1.0, double offset = 1.0, double gain = 2.0)
        {
            _gain = gain;
            _offset = offset;
            _lacunarity = lacunarity;
            _octaves = octaves;
            _baseNoise = baseBaseNoise;
            _exponentArray = InitializeExponentArray(octaves, lacunarity, H);
        }

        private double[] InitializeExponentArray(int octaves, double lacunarity, double H)
        {
            var exponentArray = new double[octaves + 1];

            var frequency = 1.0;
            for (var i = 0; i <= octaves; i++)
            {
                /* compute weight for each frequency */
                exponentArray[i] = Math.Pow(frequency, -H);
                frequency *= lacunarity;
            }

            return exponentArray;
        }

        public double Noise(double x, double y)
        {
            return Noise(x, y, 0);
        }

        public double Noise(double x, double y, double z)
        {
            return Noise(new Vector3d(x, y, z));
        }

        private double Noise3(Vector3d vector)
        {
            return _baseNoise.Noise(vector.X, vector.Y, vector.Z);
        }

        private double Noise(Vector3d point)
        {
            /* get first octave */
            var signal = Noise3(point);
            /* get absolute value of signal (this creates the ridges) */
            if (signal < 0.0) signal = -signal;
            /* invert and translate (note that "offset" should be ~= 1.0) */
            signal = _offset - signal;
            /* square the signal, to increase "sharpness" of ridges */
            signal *= signal;
            /* assign initial values */
            var result = signal;

            for (var i = 1; i < _octaves; i++)
            {
                /* increase the frequency */
                point.X *= _lacunarity;
                point.Y *= _lacunarity;
                point.Z *= _lacunarity;

                /* weight successive contributions by previous signal */
                var weight = signal * _gain;
                if (weight > 1.0) weight = 1.0;
                if (weight < 0.0) weight = 0.0;
                signal = Noise3(point);
                if (signal < 0.0) signal = -signal;
                signal = _offset - signal;
                signal *= signal;
                /* weight the contribution */
                signal *= weight;
                result += signal * _exponentArray[i];
            }

            return (result);
        }
    }
}