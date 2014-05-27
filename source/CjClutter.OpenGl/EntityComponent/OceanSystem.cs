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
        public Mesh3V3N Mesh { get; set; }

        public OceanComponent(Mesh3V3N mesh)
        {
            Mesh = mesh;
        }
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
                        ModelMatrix = Matrix4.Identity
                    };
                    entityManager.AddComponentToEntity(water, waterMesh);
                }

                var mesh = CreateMesh(waterComponent);
                waterMesh.Update(mesh);
                for (var i = 0; i < waterMesh.Mesh.Vertices.Length; i++)
                {
                    var position = waterMesh.Mesh.Vertices[i].Position;
                    var vector3D = Gerstner2.CalculateWave((Vector3d)position, elapsedTime, _waveSettings);

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
            
            return new Mesh3V3N(oceanComponent.Mesh.Vertices, oceanComponent.Mesh.Faces);
        }
    }
}