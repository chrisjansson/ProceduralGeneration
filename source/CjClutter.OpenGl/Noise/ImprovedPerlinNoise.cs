using System;

namespace CjClutter.OpenGl.Noise
{
    public class ImprovedPerlinNoise : INoiseGenerator
    {
        public ImprovedPerlinNoise()
        {
            for (var i = 0; i < 256; i++)
            {
                _p[i] = _permutation[i];
                _p[256 + i] = _permutation[i];
            }
        }

        public ImprovedPerlinNoise(int seed)
        {
            var random = new Random(seed);

            for (var i = 0; i < 256; i++)
            {
                _permutation[i] = i;
            }

            for (var i = 0; i < 256; i++)
            {
                var k = random.Next(256 - i) + i;
                var l = _permutation[i];

                _permutation[i] = _permutation[k];
                _permutation[k] = l;
            }

            for (var i = 0; i < 256; i++)
            {
                _p[i] = _permutation[i];
                _p[i + 256] = _permutation[i];
            }
        }

        public double Noise(double x, double y)
        {
            return Noise(x, y, 0);
        }

        public double Noise(double x, double y, double z)
        {
            // FIND UNIT CUBE THAT
            // CONTAINS POINT.
            var X = (int)Math.Floor(x) & 255;
            var Y = (int)Math.Floor(y) & 255;
            var Z = (int)Math.Floor(z) & 255;

            x -= Math.Floor(x);                                // FIND RELATIVE X,Y,Z
            y -= Math.Floor(y);                                // OF POINT IN CUBE.
            z -= Math.Floor(z);

            // COMPUTE FADE CURVES
            // FOR EACH OF X,Y,Z.
            var u = Fade(x);
            var v = Fade(y);
            var w = Fade(z);

            // HASH COORDINATES OF
            // THE 8 CUBE CORNERS.
            var a = _p[X] + Y;
            var aa = _p[a] + Z;
            var ab = _p[a + 1] + Z;
            var b = _p[X + 1] + Y;
            var ba = _p[b] + Z;
            var bb = _p[b + 1] + Z;

            return Lerp(w, Lerp(v, Lerp(u, Grad(_p[aa], x, y, z),  // AND ADD
                                           Grad(_p[ba], x - 1, y, z)), // BLENDED
                                   Lerp(u, Grad(_p[ab], x, y - 1, z),  // RESULTS
                                           Grad(_p[bb], x - 1, y - 1, z))),// FROM  8
                           Lerp(v, Lerp(u, Grad(_p[aa + 1], x, y, z - 1),  // CORNERS
                                           Grad(_p[ba + 1], x - 1, y, z - 1)), // OF CUBE
                                   Lerp(u, Grad(_p[ab + 1], x, y - 1, z - 1),
                                           Grad(_p[bb + 1], x - 1, y - 1, z - 1))));
        }

        private double Fade(double t)
        {
            return t * t * t * (t * (t * 6 - 15) + 10);
        }

        private double Lerp(double t, double a, double b)
        {
            return a + t * (b - a);
        }

        private double Grad(int hash, double x, double y, double z)
        {
            var h = hash & 15;                      // CONVERT LO 4 BITS OF HASH CODE
            var u = h < 8 ? x : y;               // INTO 12 GRADIENT DIRECTIONS.
            var v = h < 4 ? y : h == 12 || h == 14 ? x : z;

            return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
        }

        private readonly int[] _p = new int[512];
        private readonly int[] _permutation =
            {
                151,160,137,91,90,15,
                131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
                190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
                88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
                77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
                102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
                135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
                5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
                223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
                129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
                251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
                49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
                138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180
            };
    }
}