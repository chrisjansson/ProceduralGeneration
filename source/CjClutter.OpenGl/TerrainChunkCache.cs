using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using CjClutter.OpenGl.EntityComponent;
using CjClutter.OpenGl.Gui;
using CjClutter.OpenGl.SceneGraph;

namespace CjClutter.OpenGl
{
    public class TerrainChunkCache
    {
        private readonly ConcurrentDictionary<Box3D, RenderableMesh> _cache = new ConcurrentDictionary<Box3D, RenderableMesh>();
        private readonly TerrainChunkFactory _terrainChunkFactory;
        private readonly IResourceAllocator _resourceAllocator;
        private readonly Dictionary<Box3D, Task<Mesh3V3N>> _jobs;

        public TerrainChunkCache(TerrainChunkFactory terrainChunkFactory, IResourceAllocator resourceAllocator)
        {
            _resourceAllocator = resourceAllocator;
            _terrainChunkFactory = terrainChunkFactory;
            _jobs = new Dictionary<Box3D, Task<Mesh3V3N>>();
        }

        public RenderableMesh GetRenderable(Box3D bounds)
        {
            RenderableMesh mesh;
            var result = _cache.TryGetValue(bounds, out mesh);
            if (result)
            {
                return mesh;
            }

            if (!_jobs.ContainsKey(bounds))
            {
                var inProgressTask = Task.Run(() => _terrainChunkFactory.Create(bounds));
                _jobs.Add(bounds, inProgressTask);
                return null;
            }

            var meshTask = _jobs[bounds];
            if (!meshTask.IsCompleted)
            {
                return null;
            }

            _jobs.Remove(bounds);
            var allocatedMesh = meshTask.Result;
            mesh = _resourceAllocator.AllocateResourceFor(allocatedMesh);
            _cache[bounds] = mesh;

            return mesh;
        }
    }
}