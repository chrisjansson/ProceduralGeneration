namespace CjClutter.OpenGl.Noise
{
    interface INoiseGenerator
    {
        double Noise(double x, double y);
        double Noise(double x, double y, double z);
    }
}
