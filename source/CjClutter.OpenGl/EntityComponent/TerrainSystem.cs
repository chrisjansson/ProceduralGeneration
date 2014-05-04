using CjClutter.OpenGl.Noise;
using CjClutter.OpenGl.SceneGraph;

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
        private FractalBrownianMotionSettings _fractalBrownianMotionSettings = FractalBrownianMotionSettings.Default;
        private bool _settingsChanged = true;

        public TerrainSystem(FractalBrownianMotionSettings terrainSettings)
        {
            _fractalBrownianMotionSettings = terrainSettings;
        }

        public void SetTerrainSettings(FractalBrownianMotionSettings terrainSettings)
        {
            _settingsChanged = true;
            _fractalBrownianMotionSettings = terrainSettings;
        }

        public void Update(double elapsedTime, EntityManager entityManager)
        {
            if (!_settingsChanged)
            {
                return;
                
            }
            var terrainGenerator = new TerrainGenerator(_fractalBrownianMotionSettings);

            foreach (var entity in entityManager.GetEntitiesWithComponent<ChunkComponent>())
            {
                var chunk = entityManager.GetComponent<ChunkComponent>(entity);
                var staticMesh = entityManager.GetComponent<StaticMesh>(entity);

                terrainGenerator.GenerateMesh(staticMesh, chunk.X, chunk.Y, 10, 10);
            }

            _settingsChanged = false;
        }
    }
}