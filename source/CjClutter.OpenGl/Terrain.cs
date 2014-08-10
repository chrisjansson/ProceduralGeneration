using System;
using CjClutter.OpenGl.Camera;
using CjClutter.OpenGl.EntityComponent;
using CjClutter.OpenGl.Noise;
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

            var levels = (int)Math.Log(4096, 2);
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

            //get mesh
            //upload mesh

            //create a number of renderjobs
        }
    }

    public class TerrainChunkFactory
    {
        public void Create(Box3D bounds)
        {
            int meshDimensions = 10;
            var mesh = MeshCreator.CreateFromHeightMap(meshDimensions, meshDimensions, null);
        }

        public class ImplicitChunkHeightMap : IHeightMap
        {
            private Box3D _bounds;
            private readonly INoiseGenerator _noiseGenerator;
            private readonly int _columns;
            private readonly int _rows;

            public ImplicitChunkHeightMap(Box3D bounds, int columns, int rows, INoiseGenerator noiseGenerator)
            {
                _noiseGenerator = noiseGenerator;
                _rows = rows;
                _columns = columns;
                _bounds = bounds;
            }

            public double GetHeight(int column, int row)
            {
                var position = CalculatePosition(column, row);
                return _noiseGenerator.Noise(position.X, position.Y);
            }

            public Vector3d GetNormal(int column, int row)
            {
                var center = CalculatePosition(column, row);
                var right = new Vector3d(center.X + 1, center.Y, _noiseGenerator.Noise(center.X + 1, center.Y));
                var left = new Vector3d(center.X - 1, center.Y, _noiseGenerator.Noise(center.X - 1, center.Y));
                var top = new Vector3d(center.X, center.Y + 1, _noiseGenerator.Noise(center.X, center.Y + 1));
                var bottom = new Vector3d(center.X, center.Y - 1, _noiseGenerator.Noise(center.X, center.Y - 1));

                return Vector3d.Cross(right - left, top - bottom).Normalized();
            }

            private Vector2d CalculatePosition(int column, int row)
            {
                var delta = _bounds.Max - _bounds.Min;

                var columnFraction = column / (double)_columns;
                var rowFraction = row / (double)_rows;

                var x = _bounds.Min.X + delta.X * columnFraction;
                var y = _bounds.Min.Y + delta.Y * rowFraction;

                return new Vector2d(x, y);
            }
        }
    }
}

