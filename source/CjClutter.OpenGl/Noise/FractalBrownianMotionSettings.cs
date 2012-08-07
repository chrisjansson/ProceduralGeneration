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
}