namespace CjClutter.OpenGl.Noise
{
    public class FractalBrownianMotion
    {
        private const double Lacunarity = 2.0; //1.8715 or 2.1042 can help reduce artifacts in some algorithms
        private const double Gain = 0.5; //Generally 1/Lacunarity

        private readonly ImprovedPerlinNoise _noiseGenerator;
        private readonly FractalBrownianMotionSettings _settings;

        public FractalBrownianMotion(ImprovedPerlinNoise noiseGenerator, FractalBrownianMotionSettings settings)
        {
            _settings = settings;
            _noiseGenerator = noiseGenerator;
        }

        public double Noise(double x, double y, double z)
        {
            var frequency = _settings.Frequency;
            var amplitude = _settings.Amplitude;
            var octaves = _settings.Octaves;

            var result = 0.0;

            for (var i = 0; i < octaves; i++)
            {
                result += _noiseGenerator.Noise(x * frequency, y * frequency, z * frequency) * amplitude;
                frequency *= Lacunarity;
                amplitude *= Gain;
            }

            return result;
        }
    }
}