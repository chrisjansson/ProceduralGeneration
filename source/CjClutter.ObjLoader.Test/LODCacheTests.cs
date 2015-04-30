using System;
using System.Collections.Generic;
using CjClutter.OpenGl;
using CjClutter.OpenGl.EntityComponent;
using NUnit.Framework;

namespace ObjLoader.Test
{
    public class LodCacheTests
    {
        private List<ChunkedLodTreeFactory.ChunkedLodTreeNode> _cache;

        [SetUp]
        public void SetUp()
        {
            _cache = new List<ChunkedLodTreeFactory.ChunkedLodTreeNode>();
        }

        [Test]
        public void No_nodes_are_requested_or_to_cache_when_none_are_visible()
        {
            var cache = GetCache();

            var result = LODCache.getNodesToDrawAndCache(cache, new ChunkedLodTreeFactory.ChunkedLodTreeNode[] { });

            Assert.IsEmpty(result.Item1);
            Assert.IsEmpty(result.Item2);
        }

        [Test]
        public void No_nodes_are_requested_if_the_root_is_not_cached()
        {
            var root = new ChunkedLodTreeFactory.ChunkedLodTreeNode(new Bounds2D(), null, 0);
            var cache = GetCache();

            var result = LODCache.getNodesToDrawAndCache(cache, new ChunkedLodTreeFactory.ChunkedLodTreeNode[] { root });

            Assert.IsEmpty(result.Item1);
            CollectionAssert.AreEqual(new[] { root }, result.Item2);
        }

        [Test]
        public void No_nodes_are_requested_if_the_root_is_not_cached_when_requesting_child()
        {
            var root = new ChunkedLodTreeFactory.ChunkedLodTreeNode(new Bounds2D(), null, 0);
            var child = new ChunkedLodTreeFactory.ChunkedLodTreeNode(new Bounds2D(), root, 0);
            var cache = GetCache();

            var result = LODCache.getNodesToDrawAndCache(cache, new[] { child });

            Assert.IsEmpty(result.Item1);
            CollectionAssert.AreEqual(new[] { root, child }, result.Item2);
        }

        [Test]
        public void All_nodes_are_requested_when_all_are_cached()
        {
            var node0 = new ChunkedLodTreeFactory.ChunkedLodTreeNode(new Bounds2D(), null, 0);
            var node1 = new ChunkedLodTreeFactory.ChunkedLodTreeNode(new Bounds2D(), node0, 0);
            var node2 = new ChunkedLodTreeFactory.ChunkedLodTreeNode(new Bounds2D(), node1, 0);
            var node3 = new ChunkedLodTreeFactory.ChunkedLodTreeNode(new Bounds2D(), null, 0);
            CacheNodes(node0, node1, node2, node3);

            var cache = GetCache();
            var result = LODCache.getNodesToDrawAndCache(cache, new[] { node0, node1, node2, node3 });

            CollectionAssert.AreEqual(new[] { node0, node1, node2, node3 }, result.Item1);
            Assert.IsEmpty(result.Item2);
        }

        [Test]
        public void Requests_nodes_parent_if_originally_requested_node_is_not_cached_but_when_parent_is()
        {
            var parent = new ChunkedLodTreeFactory.ChunkedLodTreeNode(new Bounds2D(), null, 0);
            var child = new ChunkedLodTreeFactory.ChunkedLodTreeNode(new Bounds2D(), parent, 0);
            CacheNode(parent);

            var cache = GetCache();
            var result = LODCache.getNodesToDrawAndCache(cache, new[] { child });

            CollectionAssert.AreEqual(new[] { parent }, result.Item1);
            CollectionAssert.AreEqual(new[] { child }, result.Item2);
        }

        [Test]
        public void Does_not_request_child_unless_all_children_are_cached()
        {
            var parent = new ChunkedLodTreeFactory.ChunkedLodTreeNode(new Bounds2D(), null, 0);
            var firstChild = new ChunkedLodTreeFactory.ChunkedLodTreeNode(new Bounds2D(), parent, 0);
            var secondChild = new ChunkedLodTreeFactory.ChunkedLodTreeNode(new Bounds2D(), parent, 0);
            CacheNode(parent);
            CacheNode(firstChild);

            var cache = GetCache();
            var result = LODCache.getNodesToDrawAndCache(cache, new[] { firstChild, secondChild });

            CollectionAssert.AreEqual(new[] { parent }, result.Item1);
            CollectionAssert.AreEqual(new[] { secondChild }, result.Item2);
        }

        [Test]
        public void Do_not_request_descendants_when_the_ancestor_is_also_requested_even_when_the_descendant_is_cached()
        {
            var level0 = new ChunkedLodTreeFactory.ChunkedLodTreeNode(new Bounds2D(), null, 0);
            var level10 = new ChunkedLodTreeFactory.ChunkedLodTreeNode(new Bounds2D(), level0, 0);
            var level11 = new ChunkedLodTreeFactory.ChunkedLodTreeNode(new Bounds2D(), level0, 0);
            var level20 = new ChunkedLodTreeFactory.ChunkedLodTreeNode(new Bounds2D(), level10, 0);

            CacheNode(level0);
            CacheNode(level20);

            var cache = GetCache();
            var result = LODCache.getNodesToDrawAndCache(cache, new[] { level11, level20 });

            CollectionAssert.AreEqual(new[] { level0 }, result.Item1);
            CollectionAssert.AreEqual(new[] { level11 }, result.Item2);
        }

        [Test]
        public void Do_not_request_descendants_when_the_ancestor_is_also_requested()
        {
            var level0 = new ChunkedLodTreeFactory.ChunkedLodTreeNode(new Bounds2D(), null, 0);
            var level10 = new ChunkedLodTreeFactory.ChunkedLodTreeNode(new Bounds2D(), level0, 0);
            var level11 = new ChunkedLodTreeFactory.ChunkedLodTreeNode(new Bounds2D(), level0, 0);
            var level20 = new ChunkedLodTreeFactory.ChunkedLodTreeNode(new Bounds2D(), level10, 0);

            CacheNode(level0);

            var cache = GetCache();
            var result = LODCache.getNodesToDrawAndCache(cache, new[] { level11, level20 });

            CollectionAssert.AreEqual(new[] { level0 }, result.Item1);
            CollectionAssert.AreEqual(new[] { level11, level20 }, result.Item2);
        }

        private LODCache.LODCache GetCache()
        {
            Func<ChunkedLodTreeFactory.ChunkedLodTreeNode, bool> contains = node => _cache.Contains(node);
            Action<ChunkedLodTreeFactory.ChunkedLodTreeNode> beginCache = _ => { };
            Func<ChunkedLodTreeFactory.ChunkedLodTreeNode, Rendering.AllocatedMesh> get = _ => { throw new Exception(); };
            var a = FSharpUtil.FSharpFuncUtil.ToFSharpFunc(contains);
            var b = FSharpUtil.FSharpFuncUtil.ToFSharpFunc(beginCache);
            var c = FSharpUtil.FSharpFuncUtil.ToFSharpFunc(get);
            var cache = new LODCache.LODCache(
                a,
                b,
                c);

            return cache;
        }

        private void CacheNodes(params ChunkedLodTreeFactory.ChunkedLodTreeNode[] nodes)
        {
            foreach (var chunkedLodTreeNode in nodes)
            {
                CacheNode(chunkedLodTreeNode);
            }
        }

        private void CacheNode(ChunkedLodTreeFactory.ChunkedLodTreeNode node)
        {
            _cache.Add(node);
        }
    }
}