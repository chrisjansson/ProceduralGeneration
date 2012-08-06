namespace CjClutter.OpenGl.Noise
{
    public class FractalBrownianMotionSettings
    {
        public FractalBrownianMotionSettings(int octaves, double amplitude, double frequency)
        {
            Octaves = octaves;
            Amplitude = amplitude;
            Frequency = frequency;
        }

        public int Octaves { get; private set; }
        public double Amplitude { get; private set; }
        public double Frequency { get; private set; }
    }

    public class FractalBrownianMotion
    {
        private const double Lacunarity = 2.0;
        private const double Gain = 0.5;

        private readonly ImprovedPerlinNoise _noiseGenerator;
        private FractalBrownianMotionSettings _settings;

        public FractalBrownianMotion(ImprovedPerlinNoise noiseGenerator, FractalBrownianMotionSettings settings)
        {
            _settings = settings;
            _noiseGenerator = noiseGenerator;
        }

        public double Noise(double x, double y, double z)
        {
            var total = 0.0;
            var frequency = _settings.Frequency;
            var amplitude = Gain;

            for (var i = 0; i < _settings.Octaves; i++)
            {
                total += _noiseGenerator.Noise(x * frequency, y * frequency, z * frequency) * amplitude;
                frequency *= Lacunarity;
                amplitude *= Gain;
            }

            return total;
        }
    }
}