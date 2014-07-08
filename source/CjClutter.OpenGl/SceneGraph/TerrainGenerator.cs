using CjClutter.OpenGl.EntityComponent;
using CjClutter.OpenGl.Noise;
using OpenTK;

namespace CjClutter.OpenGl.SceneGraph
{
    public class NoiseFactory
    {
        public class NoiseParameters
        {
            public double Amplitude { get; set; }
            public double Frequency { get; set; }
            public int Octaves { get; set; }
            public double Lacunarity { get; set; }
            public double H { get; set; }
            public double Offset { get; set; }
            public double Gain { get; set; }
        }

        public abstract class NoiseType
        {
            public abstract INoiseGenerator Create(NoiseParameters noiseParameters);
        }

        public class RidgedMultiFractal : NoiseType
        {
            public override INoiseGenerator Create(NoiseParameters noiseParameters)
            {
                return new Noise.RidgedMultiFractal(
                    new SimplexNoise(), 
                    noiseParameters.Octaves,
                    noiseParameters.Lacunarity,
                    noiseParameters.H,
                    noiseParameters.Offset,
                    noiseParameters.Gain);
            }
        }
    }

    public class TerrainGenerator
    {
        private readonly INoiseGenerator _noise;
        private readonly ColorCycle _colorCycle;

        public TerrainGenerator(INoiseGenerator noiseGenerator)
        {
            //_noise = new FractalBrownianMotion(new SimplexNoise(), settings);
            _noise = noiseGenerator;//new RidgedMultiFractal(new SimplexNoise(), 7, 2.1347);
            _colorCycle = new ColorCycle();
        }

        public void GenerateMesh(StaticMesh staticMesh, int x, int y, float numberOfChunksX, float numberOfChunksY)
        {
            var offset = new Vector2(x, y);

            var noiseGenerator = new ChunkNoiseGenerator(offset, _noise);

            var heightMapGenerator = new HeightMapGenerator(noiseGenerator);
            var mesh = heightMapGenerator.GenerateMesh();

            var translationMatrix = Matrix4.CreateTranslation(offset.X - numberOfChunksX / 2.0f, 0, offset.Y - numberOfChunksY / 2.0f);
            staticMesh.ModelMatrix = translationMatrix;
            staticMesh.Color = _colorCycle.GetNext();
            staticMesh.Update(mesh);
        }
    }
}