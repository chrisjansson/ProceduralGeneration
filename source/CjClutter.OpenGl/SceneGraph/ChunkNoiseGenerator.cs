using CjClutter.OpenGl.Noise;
using OpenTK;

namespace CjClutter.OpenGl.SceneGraph
{
    public class ChunkNoiseGenerator : INoiseGenerator
    {
        private readonly INoiseGenerator _noiseGenerator;
        private Vector2 _offset;

        public ChunkNoiseGenerator(Vector2 offset, INoiseGenerator noiseGenerator)
        {
            _offset = offset;
            _noiseGenerator = noiseGenerator;
        }

        public double Noise(double x, double y)
        {
            return _noiseGenerator.Noise(x + _offset.X, y + _offset.Y);
        }

        public double Noise(double x, double y, double z)
        {
            throw new System.NotImplementedException();
        }
    }
}