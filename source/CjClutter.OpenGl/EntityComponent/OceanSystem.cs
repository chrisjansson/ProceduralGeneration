using System.Collections.Generic;
using CjClutter.OpenGl.Noise;
using CjClutter.OpenGl.OpenGl.VertexTypes;
using CjClutter.OpenGl.SceneGraph;
using OpenTK;

namespace CjClutter.OpenGl.EntityComponent
{
    public class OceanComponent : IEntityComponent
    {
        public OceanComponent(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public int Width { get; private set; }
        public int Height { get; private set; }
    }

    public class OceanSystem : IEntitySystem
    {
        private readonly INoiseGenerator _improvedPerlinNoise = new FractalBrownianMotion(new SimplexNoise(), FractalBrownianMotionSettings.Default);

        public void Update(double elapsedTime, EntityManager entityManager)
        {
            foreach (var water in entityManager.GetEntitiesWithComponent<OceanComponent>())
            {
                var waterComponent = entityManager.GetComponent<OceanComponent>(water);
                var waterMesh = entityManager.GetComponent<StaticMesh>(water);
                if (waterMesh == null)
                {
                    waterMesh = new StaticMesh();
                    var mesh3V3N = CreateMesh(waterComponent);
                    mesh3V3N.CalculateNormals();
                    waterMesh.Update(mesh3V3N);
                    waterMesh.Color = new Vector4(0f, 0f, 1f, 0f);
                    waterMesh.ModelMatrix = Matrix4.CreateTranslation(5, 0, -5);
                    entityManager.AddComponentToEntity(water, waterMesh);
                }

                for (var i = 0; i < waterMesh.Mesh.Vertices.Length; i++)
                {
                    var position = waterMesh.Mesh.Vertices[i].Position;
                    var height = (float)_improvedPerlinNoise.Noise(position.X, position.Z, elapsedTime / 10) / 10;
                    waterMesh.Mesh.Vertices[i] = new Vertex3V3N
                    {
                        Position = new Vector3(position.X, height, position.Z)
                    };
                }
                waterMesh.Mesh.CalculateNormals();
                waterMesh.Update(waterMesh.Mesh);
            }
        }

        private static Mesh3V3N CreateMesh(OceanComponent oceanComponent)
        {
            var vertices = new List<Vertex3V3N>();
            for (var i = 0; i <= oceanComponent.Width; i++)
            {
                for (var j = 0; j <= oceanComponent.Height; j++)
                {
                    var xin = i / (double)oceanComponent.Width * 10;
                    var yin = j / (double)oceanComponent.Height * 10;

                    var position = new Vector3((float)xin, 0, (float)yin);
                    var vertex = new Vertex3V3N { Position = position };
                    vertices.Add(vertex);
                }
            }

            var faces = new List<Face3>();
            for (var i = 0; i < oceanComponent.Width; i++)
            {
                for (var j = 0; j < oceanComponent.Height; j++)
                {
                    var verticesInColumn = (oceanComponent.Height + 1);
                    var v0 = i * verticesInColumn + j;
                    var v1 = (i + 1) * verticesInColumn + j;
                    var v2 = (i + 1) * verticesInColumn + j + 1;
                    var v3 = i * verticesInColumn + j + 1;

                    var f0 = new Face3 { V0 = v0, V1 = v1, V2 = v2 };
                    var f1 = new Face3 { V0 = v0, V1 = v2, V2 = v3 };

                    faces.Add(f0);
                    faces.Add(f1);
                }
            }

            return new Mesh3V3N(vertices, faces);
        }
    }
}