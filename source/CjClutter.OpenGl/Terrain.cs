using System;
using CjClutter.OpenGl.Camera;
using CjClutter.OpenGl.EntityComponent;
using OpenTK;

namespace CjClutter.OpenGl
{
    public class Terrain
    {
        private readonly ChunkedLodTreeFactory.ChunkedLodTreeNode _tree;
        private readonly ChunkedLod _chunkedLod;

        public Terrain()
        {
            _tree = CreateTree();
            _chunkedLod = new ChunkedLod();
        }

        private static ChunkedLodTreeFactory.ChunkedLodTreeNode CreateTree()
        {
            var chunkedLodTreeFactory = new ChunkedLodTreeFactory();

            var bounds = new Box3D(
                new Vector3d(-2048, -2048, 0),
                new Vector3d(2048, 2048, 0));

            var levels = (int) Math.Log(4096, 2);
            return chunkedLodTreeFactory.Create(bounds, levels);
        }

        public void Render(ICamera camera)
        {
            var visibleChunks = _chunkedLod.Calculate(
                _tree,
                camera.Width,
                camera.HorizontalFieldOfView,
                camera.Position,
                100);


        }
    }
}