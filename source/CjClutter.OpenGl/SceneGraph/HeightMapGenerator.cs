using System.Collections.Generic;
using CjClutter.OpenGl.Noise;
using CjClutter.OpenGl.OpenGl.VertexTypes;
using OpenTK;

namespace CjClutter.OpenGl.SceneGraph
{
    public class HeightMapGenerator
    {
        private readonly INoiseGenerator _noiseGenerator;
        private const int TerrainWidth = 32;
        private const int TerrainHeight = 32;

        public HeightMapGenerator(INoiseGenerator noiseGenerator)
        {
            _noiseGenerator = noiseGenerator;
        }

        public Mesh3V3N GenerateMesh()
        {
            var vertex3V3Ns = new List<Vertex3V3N>();
            for (var i = -1; i <= TerrainWidth + 1; i++)
            {
                for (var j = -1; j <= TerrainHeight + 1; j++)
                {
                    var vertex3V3N = CalculateVertex(i, j);
                    vertex3V3Ns.Add(vertex3V3N);
                }
            }
            var tempMesh = new Mesh3V3N(vertex3V3Ns, CreateFacesForColumnMajorList(TerrainWidth + 2, TerrainHeight + 2));
            tempMesh.CalculateNormals();

            var vertices = new List<Vertex3V3N>();
            for (var i = 0; i <= TerrainWidth; i++)
            {
                for (var j = 0; j <= TerrainHeight; j++)
                {
                    var offset = (TerrainHeight + 3)*(i + 1) + j + 1;
                    vertices.Add(tempMesh.Vertices[offset]);
                }
            }

            var mesh3V3N = new Mesh3V3N(vertices, CreateFacesForColumnMajorList(TerrainWidth, TerrainHeight));
            return mesh3V3N;
        }

        private Vertex3V3N CalculateVertex(int xin, int yin)
        {
            var x = xin / (double)TerrainWidth;
            var z = yin / (double)TerrainHeight;
            var heigt = 0.2 * _noiseGenerator.Noise(x, z);

            var position = new Vector3((float)x, (float)heigt, (float)z);
            return new Vertex3V3N { Position = position };
        }

        private IEnumerable<Face3> CreateFacesForColumnMajorList(int width, int height)
        {
            var faces = new List<Face3>();
            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    var verticesInColumn = height + 1;
                    var v0 = i * verticesInColumn + j;
                    var v1 = (i + 1) * verticesInColumn + j;
                    var v2 = (i + 1) * verticesInColumn + (j + 1);
                    var v3 = i * verticesInColumn + (j + 1);

                    var f0 = new Face3 { V0 = v0, V1 = v1, V2 = v2 };
                    var f1 = new Face3 { V0 = v0, V1 = v2, V2 = v3 };

                    faces.Add(f0);
                    faces.Add(f1);
                }
            }

            return faces;
        }
    }
}