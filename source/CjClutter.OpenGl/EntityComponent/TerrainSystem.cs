using CjClutter.OpenGl.Noise;
using CjClutter.OpenGl.SceneGraph;
using OpenTK;

namespace CjClutter.OpenGl.EntityComponent
{
    public class ChunkComponent : IEntityComponent
    {
        public ChunkComponent(int x, int y)
        {
            X = x;
            Y = y;
        }
        
        public int X { get; private set; }
        public int Y { get; private set; }
    }

    public class TerrainSystem : IEntitySystem
    {
        private bool _settingsChanged = true;
        private INoiseGenerator _noiseGenerator;

        public TerrainSystem(FractalBrownianMotionSettings terrainSettings)
        {
            _noiseGenerator = new ImprovedPerlinNoise();
        }

        public void SetTerrainSettings(INoiseGenerator noiseGenerator)
        {
            _noiseGenerator = noiseGenerator;
            _settingsChanged = true;
        }

        public void Update(double elapsedTime, EntityManager entityManager)
        {
            if (!_settingsChanged)
            {
                return;
                
            }
            var terrainGenerator = new TerrainGenerator(_noiseGenerator);

            foreach (var entity in entityManager.GetEntitiesWithComponent<ChunkComponent>())
            {
                var chunk = entityManager.GetComponent<ChunkComponent>(entity);
                var staticMesh = entityManager.GetComponent<StaticMesh>(entity);

                terrainGenerator.GenerateMesh(staticMesh, chunk.X, chunk.Y, 10, 10);
            }

            _settingsChanged = false;
        }
    }

    public class CubeMeshComponent : IEntityComponent
    {
        public Matrix4 Transform { get; set; }

        public CubeMeshComponent(Matrix4 transform)
        {
            Transform = transform;
        }
    }

    public class CubeMeshSystem : IEntitySystem
    {
        public void Update(double elapsedTime, EntityManager entityManager)
        {
            foreach (var entity in entityManager.GetEntitiesWithComponent<CubeMeshComponent>())
            {
                var mesh = entityManager.GetComponent<StaticMesh>(entity);
                if (mesh == null)
                {
                    var component = entityManager.GetComponent<CubeMeshComponent>(entity);    
                    mesh = new StaticMesh();
                    mesh.ModelMatrix = component.Transform;
                    mesh.Color = new Vector4(0f, 0f, 1f, 1f);
                    mesh.Update(MeshCreator.CreateXZGrid(25, 25));
                    
                    entityManager.AddComponentToEntity(entity, mesh);
                }
            }
        }
    }
}