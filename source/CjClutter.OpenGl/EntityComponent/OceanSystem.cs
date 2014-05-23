using System;
using System.Collections.Generic;
using System.Linq;
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
        public static Vector3d A(List<WaveSetting> settings, Vector2d position, double time)
        {
            var offset = settings
                .Select(x => CalculateOffset(x, position, time))
                .Aggregate((x, y) => x + y);

            var height = settings
                .Select(x => CalculateHeight(x, position, time))
                .Sum();

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

    public class Gerstner2
    {
        public class Settings
        {
            public Vector2d Direction { get; set; }
            public double WaveLength { get; set; }
            public double Steepness { get; set; }
            public double Frequency { get; set; }
            public double Phase { get; set; }
        }


        public static Vector3d CalculateWave(Vector3d position, double time, Settings[] settings)
        {
            var sum = new Vector3d();
            for (var i = 0; i < settings.Length; i++)
            {
                sum += CalculateWave(position.Xz, time, settings[i]);
            }

            return new Vector3d(position.X, position.Y, position.Z) - new Vector3d(sum.X, -sum.Y, sum.Z);
        }

        public static Vector3d CalculateWave(Vector2d position, double time, Settings settings)
        {
            var magnitude = (Math.PI * 2) / settings.WaveLength;
            var k = settings.Direction * magnitude;
            var amplitude = settings.Steepness / magnitude;
            var frequency = Math.Sqrt(9.82 * magnitude);
            var phase = settings.Phase;

            var offset = (k / magnitude) * amplitude * Math.Sin(Vector2d.Dot(k, position) - frequency * time + phase);
            var height = amplitude * Math.Cos(Vector2d.Dot(k, position) - frequency * time + phase);
            return new Vector3d(offset.X, height, offset.Y);
        }
    }

    public class OceanSystem : IEntitySystem
    {
        private readonly INoiseGenerator _improvedPerlinNoise = new FractalBrownianMotion(new SimplexNoise(), FractalBrownianMotionSettings.Default);
        private Gerstner2.Settings[] _waveSettings;

        public OceanSystem()
        {
            var windSpeed = 2;
            var waveHeight = 0.21 * windSpeed * windSpeed / 9.82;
            var waveLength = (waveHeight * Math.PI * 2) / 0.9;
            _waveSettings = CreateWaves(Math.PI * 0.3, waveLength, 0.002, 0.2).ToArray();
        }

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
                        //ModelMatrix = Matrix4.CreateTranslation(-5, 0, -5)
                        ModelMatrix = Matrix4.Identity
                    };
                    entityManager.AddComponentToEntity(water, waterMesh);
                }

                //var waveSetting0 = new Gerstner.WaveSetting
                //{
                //    Amplitude = 0.2,
                //    Direction = new Vector2d(0.2, 0.4),
                //    Frequency = CalculateFrequency(10),
                //    PhaseConstant = 1,
                //    Q = 0.5
                //};

                //var waveSetting1 = new Gerstner.WaveSetting
                //{
                //    Amplitude = 0.1,
                //    Direction = new Vector2d(0.25, 0.5),
                //    Frequency = CalculateFrequency(2),
                //    PhaseConstant = 1,
                //    Q = 0.5
                //};

                //var waveSetting2 = new Gerstner.WaveSetting
                //{
                //    Amplitude = 0.05,
                //    Direction = new Vector2d(0.1, 0.7),
                //    Frequency = CalculateFrequency(0.25),
                //    PhaseConstant = 1,
                //    Q = 0.5
                //};




                //var wave1 = new Gerstner2.Settings()
                //{
                //    Steepness = 0.4,
                //    Direction = GetDirection(Math.PI / 3),
                //    WaveLength = waveLength,
                //    Phase = 0,
                //};


                //var wave2 = new Gerstner2.Settings()
                //{
                //    Steepness = 0.4,
                //    Direction = GetDirection(Math.PI / 3 + 0.2),
                //    WaveLength = waveLength / 2,
                //    Phase = 1,
                //};


                //var wave3 = new Gerstner2.Settings()
                //{
                //    Steepness = 0.4,
                //    Direction = GetDirection(Math.PI / 3 - 0.5),
                //    WaveLength = waveLength * 1.4,
                //    Phase = 0.2,
                //};

                var mesh = CreateMesh(waterComponent);
                waterMesh.Update(mesh);
                for (var i = 0; i < waterMesh.Mesh.Vertices.Length; i++)
                {
                    var position = waterMesh.Mesh.Vertices[i].Position;
                    var vector3D = Gerstner2.CalculateWave((Vector3d) position, elapsedTime, _waveSettings);

                    waterMesh.Mesh.Vertices[i] = new Vertex3V3N
                    {
                        Position = (Vector3)vector3D
                    };
                }
                waterMesh.Mesh.CalculateNormals();
                waterMesh.Update(waterMesh.Mesh);
            }
        }

        private List<Gerstner2.Settings> CreateWaves(double angle, double wavelength, double amplitude, double steepness)
        {
            var waveSettings = new List<Gerstner2.Settings>();
            var random = new Random(4711);
            int waves = 5;
            for (var i = 0; i < waves; i++)
            {
                var waveLengthFactor = wavelength * 2 - wavelength / 2;
                var minWaveLength = wavelength / 2;
                var nextDouble = random.NextDouble();
                var waveLength2 = minWaveLength + nextDouble * waveLengthFactor;

                var amplitudeSpan = amplitude * 2 - amplitude / 2;
                var minAmplitude = amplitude / 2;
                var amplitude2 = minAmplitude + nextDouble * amplitudeSpan;

                var frequency = CalculateFrequency(waveLength2);

                var directionRad = angle + (random.NextDouble() - 0.5);
                var direction = new Vector2d(Math.Cos(directionRad), Math.Sin(directionRad));

                //How to handle the amplitude is a matter of opinion. Although derivations of wave amplitude as a function of wavelength and current weather conditions probably exist, 
                //we use a constant (or scripted) ratio, specified at authoring time. More exactly, along with a median wavelength, the artist specifies a median amplitude. 
                //For a wave of any size, the ratio of its amplitude to its wavelength will match the ratio of the median amplitude to the median wavelength.

                var q = steepness / (frequency * amplitude2 * waves);

                var s = new Gerstner2.Settings
                {
                    Direction = direction.Normalized(),
                    WaveLength = waveLength2,
                    Frequency = frequency,
                    Steepness = steepness,
                    Phase = nextDouble,
                };

                waveSettings.Add(s);
            }

            return waveSettings;
        }

        private static double CalculateFrequency(double waveLength)
        {
            const double g = 9.82;
            return Math.Sqrt(g * Math.PI * 2 / waveLength);
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

                    var position = new Vector3((float)xin -5, 5, (float)yin-5);
                    var vertex = new Vertex3V3N
                    {
                        Position = position.Normalized() * 5
                    };
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