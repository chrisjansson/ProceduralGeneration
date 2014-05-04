using CjClutter.OpenGl.EntityComponent;
using CjClutter.OpenGl.Noise;
using OpenTK;

namespace CjClutter.OpenGl.SceneGraph
{
    public class TerrainGenerator
    {
        private readonly INoiseGenerator _noise;
        private readonly ColorCycle _colorCycle;

        public TerrainGenerator(FractalBrownianMotionSettings settings)
        {
            _noise = new FractalBrownianMotion(new SimplexNoise(), settings);
            _colorCycle = new ColorCycle();
        }

        public void GenerateMesh(StaticMesh staticMesh, int x, int y, int numberOfChunksX, int numberOfChunksY)
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