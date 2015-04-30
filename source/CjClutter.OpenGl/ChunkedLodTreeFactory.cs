using System;
using CjClutter.OpenGl.EntityComponent;
using OpenTK;

namespace CjClutter.OpenGl
{
    public class ChunkedLodTreeFactory
    {
        public ChunkedLodTreeNode Create(Bounds2D bounds, int depth)
        {
            return CreateChunkedLodTreeNode(bounds, null, depth, CalculateGeometricError(depth));
        }

        private double CalculateGeometricError(int depth)
        {
            return Math.Pow(2, depth);
        }

        private ChunkedLodTreeNode CreateChunkedLodTreeNode(Bounds2D first, ChunkedLodTreeNode parent, int nextDepth, double geometricError)
        {
            var chunkedLodTreeNode = new ChunkedLodTreeNode(first, parent, geometricError);
            chunkedLodTreeNode.SetNodes(GetLeafs(first, chunkedLodTreeNode, nextDepth));
            return chunkedLodTreeNode;
        }

        private ChunkedLodTreeNode[] GetLeafs(Bounds2D bounds, ChunkedLodTreeNode parent, int depth)
        {
            if (depth == 0)
            {
                return new ChunkedLodTreeNode[0];
            }

            var min = bounds.Min;
            var max = bounds.Max;
            var center = bounds.Center;

            var first = new Bounds2D(min, new Vector2d(center.X, center.Y));
            var second = new Bounds2D(new Vector2d(center.X, min.Y), new Vector2d(max.X, center.Y));
            var third = new Bounds2D(new Vector2d(min.X, center.Y), new Vector2d(center.X, max.Y));
            var fourth = new Bounds2D(new Vector2d(center.X, center.Y), max);
            var nextDepth = depth - 1;
            var geometricError = CalculateGeometricError(depth - 1);
            return new[]
            {
                CreateChunkedLodTreeNode(first, parent, nextDepth, geometricError),
                CreateChunkedLodTreeNode(second, parent, nextDepth, geometricError),
                CreateChunkedLodTreeNode(third, parent, nextDepth, geometricError),
                CreateChunkedLodTreeNode(fourth, parent, nextDepth, geometricError)
            };
        }

        public class ChunkedLodTreeNode
        {
            public ChunkedLodTreeNode(Bounds2D bounds, ChunkedLodTreeNode parent, double geometricError)
            {
                Bounds = bounds;
                Parent = parent;
                Nodes = new ChunkedLodTreeNode[0];
                GeometricError = geometricError;
                if (Parent == null)
                {
                    Level = 0;
                }
                else
                {
                    Level = parent.Level + 1;
                }
            }

            public Bounds2D Bounds { get; private set; }
            public ChunkedLodTreeNode Parent { get; set; }
            public ChunkedLodTreeNode[] Nodes { get; private set; }
            public double GeometricError { get; private set; }

            public int Level { get; set; }

            public void SetNodes(ChunkedLodTreeNode[] nodes)
            {
                Nodes = nodes;
            }
        }
    }
}