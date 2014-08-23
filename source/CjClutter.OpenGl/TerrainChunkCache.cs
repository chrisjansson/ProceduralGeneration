using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CjClutter.OpenGl.EntityComponent;
using CjClutter.OpenGl.Gui;
using CjClutter.OpenGl.SceneGraph;

namespace CjClutter.OpenGl
{
    public class TerrainChunkCache
    {
        private readonly ConcurrentDictionary<Box3D, RenderableMesh> _cache = new ConcurrentDictionary<Box3D, RenderableMesh>();
        private TerrainChunkFactory _terrainChunkFactory;
        private List<Box3D> _queue;
        private IResourceAllocator _resourceAllocator;
        private Dictionary<Box3D, Mesh3V3N> _mapping;

        public TerrainChunkCache(TerrainChunkFactory terrainChunkFactory, IResourceAllocator resourceAllocator)
        {
            _resourceAllocator = resourceAllocator;
            _terrainChunkFactory = terrainChunkFactory;
            _queue = new List<Box3D>();
            _mapping = new Dictionary<Box3D, Mesh3V3N>();

            for (int i = 0; i < 1; i++)
            {
                var workerThread = new Thread(() => Worker());
                workerThread.IsBackground = true;
                workerThread.Start();
            }
        }

        private void Worker()
        {
            while (true)
            {
                Box3D bounds;
                lock (this)
                {
                    if (!_queue.Any())
                    {
                        continue;
                    }

                    bounds = _queue[0];
                }

                var mesh = _terrainChunkFactory.Create(bounds);
                lock (this)
                {
                    _queue.Remove(bounds);
                    _mapping[bounds] = mesh;
                }
            }
        }

        public void LoadIfNotCachedAsync(Box3D bounds)
        {
            lock (this)
            {
                if (!_cache.ContainsKey(bounds) && !_queue.Contains(bounds))
                {
                    _queue.Insert(0, bounds);
                }
            }
        }

        public RenderableMesh GetRenderable(Box3D bounds)
        {
            RenderableMesh mesh;
            var result = _cache.TryGetValue(bounds, out mesh);
            if (result)
            {
                return mesh;
            }


            lock (this)
            {
                if (!_mapping.ContainsKey(bounds))
                {
                    return null;
                }
                var mesh2 = _mapping[bounds];
                var renderableMesh = _resourceAllocator.AllocateResourceFor(mesh2);
                _cache[bounds] = renderableMesh;
                _mapping.Remove(bounds);
            }

            return _cache[bounds];
        }
    }
}