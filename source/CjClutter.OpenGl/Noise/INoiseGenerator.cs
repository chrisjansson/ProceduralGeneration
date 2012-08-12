namespace CjClutter.OpenGl.Noise
{
    public interface INoiseGenerator
    {
        double Noise(double x, double y);
        double Noise(double x, double y, double z);
    }
}
