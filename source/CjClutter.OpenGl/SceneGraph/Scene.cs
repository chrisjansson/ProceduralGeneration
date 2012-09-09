using CjClutter.OpenGl.Noise;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace CjClutter.OpenGl.SceneGraph
{
    public class Scene
    {
        private readonly SimplexNoise _noiseGenerator;
        private readonly Vector3d[,] _heightMap;

        private const int TerrainWidth = 128;
        private const int TerrainHeight = 128;

        public Scene()
        {
            _noiseGenerator = new SimplexNoise();

            _heightMap = new Vector3d[TerrainWidth, TerrainHeight];
        }

        public void OnLoad()
        {
            for (var i = 0; i < TerrainWidth; i++)
            {
                for (var j = 0; j < TerrainHeight; j++)
                {
                    var xin = i / (double)TerrainWidth * 2;
                    var yin = j / (double)TerrainHeight * 2;
                    var y = 0.2 * _noiseGenerator.Noise(xin, yin);
                    var x = -0.5 + ScaleTo(i, TerrainWidth);
                    var z = -0.5 + ScaleTo(j, TerrainHeight);

                    _heightMap[i, j] = new Vector3d(x, y, z);
                }
            }
        }

        public void Update(double elapsedTime)
        {
        }

        public void Draw()
        {
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            GL.Begin(BeginMode.Quads);

            for (var i = 0; i < TerrainWidth - 1; i++)
            {
                for (var j = 0; j < TerrainHeight - 1; j++)
                {
                    var v0 = _heightMap[i, j];
                    var v1 = _heightMap[i + 1, j];
                    var v2 = _heightMap[i + 1, j + 1];
                    var v3 = _heightMap[i, j + 1];

                    GL.Vertex3(v0);
                    GL.Vertex3(v1);
                    GL.Vertex3(v2);
                    GL.Vertex3(v3);
                }
            }

            GL.End();
            GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);
        }

        private double ScaleTo(double value, double max)
        {
            return value / max;
        }
    }
}
