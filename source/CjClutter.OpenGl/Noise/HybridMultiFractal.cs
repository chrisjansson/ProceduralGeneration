using System;
using OpenTK;

namespace CjClutter.OpenGl.Noise
{
/* Hybrid additive/multiplicative multifractal terrain model.
 *
 * Copyright 1994 F. Kenton Musgrave 
 *
 * Some good parameter values to start with:
 *
 *      H:           0.25
 *      offset:      0.7
 */
    public class HybridMultiFractal : INoiseGenerator
    {
        private readonly INoiseGenerator _noise;
        private readonly double[] _exponentArray;
        private readonly double _offset;
        private readonly double _octaves;
        private readonly double _lacunarity;

        public HybridMultiFractal(INoiseGenerator noise, double octaves, double lacunarity, double h = 0.25, double offset = 0.7)
        {
            _lacunarity = lacunarity;
            _octaves = octaves;
            _offset = offset;
            _noise = noise;

            _exponentArray = InitializeExponentArray(octaves, lacunarity, h);
        }

        private static double[] InitializeExponentArray(double octaves, double lacunarity, double h)
        {
            var exponentArray = new double[(int)(octaves + 1)];
            var frequency = 1.0;
            for (var i = 0; i <= octaves; i++)
            {
                /* compute weight for each frequency */
                exponentArray[i] = Math.Pow(frequency, -h);
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
            return HybridMultifractal(new Vector3d(x, y, z));
        }

        private double HybridMultifractal(Vector3d point)
        {
            /* get first octave of function */
            var result = (Noise3(point) + _offset) * _exponentArray[0];
            var weight = result;
            /* increase frequency */
            point.X *= _lacunarity;
            point.Y *= _lacunarity;
            point.Z *= _lacunarity;

            /* spectral construction inner loop, where the fractal is built */
            int i;
            for (i = 1; i < _octaves; i++)
            {
                /* prevent divergence */
                if (weight > 1.0) weight = 1.0;

                /* get next higher frequency */
                var signal = (Noise3(point) + _offset) * _exponentArray[i];
                /* add it in, weighted by previous freq's local value */
                result += weight * signal;
                /* update the (monotonically decreasing) weighting value */
                /* (this is why H must specify a high fractal dimension) */
                weight *= signal;

                /* increase frequency */
                point.X *= _lacunarity;
                point.Y *= _lacunarity;
                point.Z *= _lacunarity;
            } /* for */

            /* take care of remainder in ``octaves''  */
            var remainder = _octaves - (int)_octaves;
            if (remainder > 0)
                /* ``i''  and spatial freq. are preset in loop above */
                result += remainder * Noise3(point) * _exponentArray[i];

            return (result);
        }

        private double Noise3(Vector3d point)
        {
            return _noise.Noise(point.X, point.Y, point.Z);
        }
    }
}