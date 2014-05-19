using System;
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

    public static class Gerstner
    {
        public static Vector3d A(WaveSetting setting, Vector2d position, double time)
        {
            var height = CalculateHeight(setting, position, time);
            var offset = CalculateOffset(setting, position, time);

            return new Vector3d(position.X + offset.X, height, position.Y + offset.Y);
        }

        private static double CalculateHeight(WaveSetting setting, Vector2d position, double time)
        {
            return setting.Amplitude * Math.Sin(Vector2d.Dot(setting.Frequency * setting.Direction, position) + setting.PhaseConstant * time);
        }

        private static Vector2d CalculateOffset(WaveSetting setting, Vector2d position, double time)
        {
            var periodic = Math.Cos(Vector2d.Dot(setting.Frequency * setting.Direction, position) + setting.PhaseConstant * time);
            var x = setting.Q * setting.Amplitude * setting.Direction.X * periodic;
            var y = setting.Q * setting.Amplitude * setting.Direction.Y * periodic;

            return new Vector2d(x, y);
        }

        public class WaveSetting
        {
            public double Q;
            public double Amplitude;
            public double Frequency;
            public double PhaseConstant;
            public Vector2d Direction;
        }
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
                    waterMesh = new StaticMesh
                    {
                        Color = new Vector4(0f, 0f, 1f, 0f),
                        ModelMatrix = Matrix4.CreateTranslation(5, 0, -5)
                    };
                    entityManager.AddComponentToEntity(water, waterMesh);
                }

                var waveSetting = new Gerstner.WaveSetting
                {
                    Amplitude = 0.2,
                    Direction = new Vector2d(0.2, 0.4),
                    Frequency = 10,
                    PhaseConstant = 1,
                    Q = 1
                };

                var mesh = CreateMesh(waterComponent);
                waterMesh.Update(mesh);
                for (var i = 0; i < waterMesh.Mesh.Vertices.Length; i++)
                {
                    var position = waterMesh.Mesh.Vertices[i].Position;
                    var vector3D = Gerstner.A(waveSetting, new Vector2d(position.X, position.Z), elapsedTime);

                    waterMesh.Mesh.Vertices[i] = new Vertex3V3N
                    {
                        Position = (Vector3) vector3D
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