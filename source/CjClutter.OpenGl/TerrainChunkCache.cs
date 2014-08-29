using System.Collections.Concurrent;
using System.Collections.Generic;
using CjClutter.OpenGl.EntityComponent;
using CjClutter.OpenGl.Gui;

namespace CjClutter.OpenGl
{
    public class TerrainChunkCache
    {
        private readonly ConcurrentDictionary<Box3D, RenderableMesh> _cache = new ConcurrentDictionary<Box3D, RenderableMesh>();
        private readonly TerrainChunkFactory _terrainChunkFactory;
        private readonly IResourceAllocator _resourceAllocator;
        private readonly HashSet<Box3D> _jobs;

        public TerrainChunkCache(TerrainChunkFactory terrainChunkFactory, IResourceAllocator resourceAllocator)
        {
            _resourceAllocator = resourceAllocator;
            _terrainChunkFactory = terrainChunkFactory;
            _jobs = new HashSet<Box3D>();
        }

        public RenderableMesh GetRenderable(Box3D bounds)
        {
            RenderableMesh mesh;
            var result = _cache.TryGetValue(bounds, out mesh);
            if (result)
            {
                return mesh;
            }

            if (!_jobs.Contains(bounds))
            {
                _jobs.Add(bounds);
                JobDispatcher.Instance.Enqueue(() =>
                {
                    var terrainChunk = _terrainChunkFactory.Create(bounds);
                    var renderableMesh = _resourceAllocator.AllocateResourceFor(terrainChunk);
                    _cache[bounds] = renderableMesh;
                    _jobs.Remove(bounds);
                });
            }

            return null;
        }
    }
}