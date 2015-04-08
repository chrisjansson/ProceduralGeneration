using System;
using CjClutter.OpenGl;
using CjClutter.OpenGl.EntityComponent;
using NUnit.Framework;

namespace ObjLoader.Test
{
    public class LodCacheTests
    {
        [Test]
        public void No_nodes_are_requested_or_to_cache_when_none_are_visible()
        {
            Func<ChunkedLodTreeFactory.ChunkedLodTreeNode, bool> contains = _ => false;
            Action<ChunkedLodTreeFactory.ChunkedLodTreeNode> beginCache = _ => { };
            Func<ChunkedLodTreeFactory.ChunkedLodTreeNode, Rendering.AllocatedMesh> get = _ => { throw new Exception(); };
            var a = FSharpUtil.FSharpFuncUtil.ToFSharpFunc(contains);
            var b = FSharpUtil.FSharpFuncUtil.ToFSharpFunc(beginCache);
            var c = FSharpUtil.FSharpFuncUtil.ToFSharpFunc(get);
            var cache = new LODCache.LODCache(
                a,
                b,
                c);

            var result = LODCache.getNodesToDrawAndCache(cache, new ChunkedLodTreeFactory.ChunkedLodTreeNode[] { });

            Assert.IsEmpty(result.Item1);
            Assert.IsEmpty(result.Item2);
        }

        [Test]
        public void All_nodse_are_requested_when_all_are_cached()
        {
            Func<ChunkedLodTreeFactory.ChunkedLodTreeNode, bool> contains = _ => true;
            Action<ChunkedLodTreeFactory.ChunkedLodTreeNode> beginCache = _ => { };
            Func<ChunkedLodTreeFactory.ChunkedLodTreeNode, Rendering.AllocatedMesh> get = _ => { throw new Exception(); };
            var a = FSharpUtil.FSharpFuncUtil.ToFSharpFunc(contains);
            var b = FSharpUtil.FSharpFuncUtil.ToFSharpFunc(beginCache);
            var c = FSharpUtil.FSharpFuncUtil.ToFSharpFunc(get);
            var cache = new LODCache.LODCache(
                a,
                b,
                c);

            var node0 = new ChunkedLodTreeFactory.ChunkedLodTreeNode(new Box3D(), null, 0);
            var node1 = new ChunkedLodTreeFactory.ChunkedLodTreeNode(new Box3D(), node0, 0);
            var node2 = new ChunkedLodTreeFactory.ChunkedLodTreeNode(new Box3D(), node1, 0);
            var node3 = new ChunkedLodTreeFactory.ChunkedLodTreeNode(new Box3D(), null, 0);
            var result = LODCache.getNodesToDrawAndCache(cache, new[] { node0, node1, node2, node3 });

            CollectionAssert.AreEqual(new[] { node0, node1, node2, node3 }, result.Item1);
            Assert.IsEmpty(result.Item2);
        }
    }
}