using System.Collections.Generic;
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

        public IList<SceneObject> Generate()
        {
            var meshes = new List<SceneObject>();

            const int numberOfChunksX = 10;
            const int numberOfChunksY = 10;
            for (var i = 0; i < numberOfChunksX; i++)
            {
                for (var j = 0; j < numberOfChunksX; j++)
                {
                    var offset = new Vector2(i, j);
                    var noiseGenerator = new ChunkNoiseGenerator(offset, _noise);

                    var heightMapGenerator = new HeightMapGenerator(noiseGenerator);
                    var mesh = heightMapGenerator.GenerateMesh();

                    var sceneObject = new SceneObject();
                    sceneObject.Mesh = mesh;

                    var translationMatrix = Matrix4.CreateTranslation(offset.X - numberOfChunksX / 2.0f, 0, offset.Y - numberOfChunksY / 2.0f);
                    sceneObject.ModelMatrix = translationMatrix;
                    sceneObject.Color = _colorCycle.GetNext();
                    meshes.Add(sceneObject);
                }
            }

            return meshes;
        }
    }
}