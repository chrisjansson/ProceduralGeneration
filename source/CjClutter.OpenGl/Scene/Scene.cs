using CjClutter.OpenGl.Noise;
using OpenTK.Graphics.OpenGL;

namespace CjClutter.OpenGl.Scene
{
    public class Scene
    {
        private readonly SimplexNoise _noiseGenerator;
        private readonly double[,] _heightMap;
        private double _nextDeadLine;

        private const int TerrainWidth = 256;
        private const int TerrainHeight = 256;

        public Scene()
        {
            _noiseGenerator = new SimplexNoise();

            _heightMap = new double[TerrainWidth,TerrainHeight];
        }

        public void Update(double elapsedTime)
        {
            if(elapsedTime < _nextDeadLine)
            {
                return;
            }

            _nextDeadLine = elapsedTime + 10;

            for (var i = 0; i < TerrainWidth; i++)
            {
                for (var j = 0; j < TerrainHeight; j++)
                {
                    var xin = i / (double)TerrainWidth * 2;
                    var yin = j / (double)TerrainHeight * 2;
                    _heightMap[i, j] = 0.2 * _noiseGenerator.Noise(xin, yin, elapsedTime / 10);
                }
            }
        }

        public void Draw()
        {
            for (var i = 0; i < TerrainWidth - 1; i++)
            {
                for (var j = 0; j < TerrainHeight - 1; j++)
                {
                    var x0 = -0.5 + ScaleTo(i, TerrainWidth);
                    var x1 = -0.5 + ScaleTo(i + 1, TerrainWidth);
                    var x2 = -0.5 + ScaleTo(i + 1, TerrainWidth);
                    var x3 = -0.5 + ScaleTo(i, TerrainWidth);

                    var y0 = -0.5 + ScaleTo(j, TerrainHeight);
                    var y1 = -0.5 + ScaleTo(j, TerrainHeight);
                    var y2 = -0.5 + ScaleTo(j + 1, TerrainHeight);
                    var y3 = -0.5 + ScaleTo(j + 1, TerrainHeight);

                    var z0 = _heightMap[i, j];
                    var z1 = _heightMap[i + 1, j];
                    var z2 = _heightMap[i + 1, j + 1];
                    var z3 = _heightMap[i, j + 1];

                    GL.Vertex3(x0, z0, y0);
                    GL.Vertex3(x1, z1, y1);
                    GL.Vertex3(x2, z2, y2);
                    GL.Vertex3(x3, z3, y3);
                }
            }
        }

        private double ScaleTo(double value, double max)
        {
            return value / max;
        }
    }
}
