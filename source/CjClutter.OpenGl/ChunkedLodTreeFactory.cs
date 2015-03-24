using System;
using CjClutter.OpenGl.EntityComponent;
using OpenTK;

namespace CjClutter.OpenGl
{
    public class ChunkedLodTreeFactory
    {
        public ChunkedLodTreeNode Create(Box3D bounds, int depth)
        {
            return CreateChunkedLodTreeNode(bounds, null, depth, CalculateGeometricError(depth));
        }

        private double CalculateGeometricError(int depth)
        {
            return Math.Pow(2, depth);
        }

        private ChunkedLodTreeNode CreateChunkedLodTreeNode(Box3D first, ChunkedLodTreeNode parent, int nextDepth, double geometricError)
        {
            var chunkedLodTreeNode = new ChunkedLodTreeNode(first, parent, geometricError);
            chunkedLodTreeNode.SetNodes(GetLeafs(first, chunkedLodTreeNode, nextDepth));
            return chunkedLodTreeNode;
        }

        private ChunkedLodTreeNode[] GetLeafs(Box3D bounds, ChunkedLodTreeNode parent, int depth)
        {
            if (depth == 0)
            {
                return new ChunkedLodTreeNode[0];
            }

            var min = bounds.Min;
            var max = bounds.Max;
            var center = bounds.Center;

            var first = new Box3D(min, new Vector3d(center.X, center.Y, max.Z));
            var second = new Box3D(new Vector3d(center.X, min.Y, min.Z), new Vector3d(max.X, center.Y, max.Z));
            var third = new Box3D(new Vector3d(min.X, center.Y, min.Z), new Vector3d(center.X, max.Y, max.Z));
            var fourth = new Box3D(new Vector3d(center.X, center.Y, min.Z), max);
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
            public ChunkedLodTreeNode(Box3D bounds, ChunkedLodTreeNode parent, double geometricError)
            {
                Bounds = bounds;
                Parent = parent;
                Nodes = new ChunkedLodTreeNode[0];
                GeometricError = geometricError;
            }

            public Box3D Bounds { get; private set; }
            public ChunkedLodTreeNode Parent { get; set; }
            public ChunkedLodTreeNode[] Nodes { get; private set; }
            public double GeometricError { get; private set; }

            public void SetNodes(ChunkedLodTreeNode[] nodes)
            {
                Nodes = nodes;
            }
        }
    }
}