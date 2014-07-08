namespace CjClutter.OpenGl.Noise
{
    public class NoiseDecorator : INoiseGenerator
    {
        private readonly double _amplitude;
        private readonly double _frequency;
        private readonly INoiseGenerator _noiseGenerator;

        public NoiseDecorator(double amplitude, double frequency, INoiseGenerator noiseGenerator)
        {
            _noiseGenerator = noiseGenerator;
            _frequency = frequency;
            _amplitude = amplitude;
        }

        public double Noise(double x, double y)
        {
            return Noise(x, y, 0);
        }

        public double Noise(double x, double y, double z)
        {
            return _amplitude*_noiseGenerator.Noise(x*_frequency, y*_frequency, z*_frequency);
        }
    }
}