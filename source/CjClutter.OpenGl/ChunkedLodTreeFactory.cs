using CjClutter.OpenGl.EntityComponent;
using OpenTK;

namespace CjClutter.OpenGl
{
    public class ChunkedLodTreeFactory
    {
        public ChunkedLodTreeNode Create(Box3D bounds, int depth)
        {
            return new ChunkedLodTreeNode(bounds, GetLeafs(bounds, depth));
        }

        private ChunkedLodTreeNode[] GetLeafs(Box3D bounds, int depth)
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
            var nextDepth = depth -1;
            return new[]
            {
                new ChunkedLodTreeNode(first, GetLeafs(first, nextDepth)),
                new ChunkedLodTreeNode(second, GetLeafs(second, nextDepth)),
                new ChunkedLodTreeNode(third, GetLeafs(third, nextDepth)),
                new ChunkedLodTreeNode(fourth, GetLeafs(fourth, nextDepth))
            };
        }

        public class ChunkedLodTreeNode
        {
            public ChunkedLodTreeNode(Box3D bounds, ChunkedLodTreeNode[] nodes)
            {
                Bounds = bounds;
                Nodes = nodes;
            }

            public Box3D Bounds { get; private set; }
            public ChunkedLodTreeNode[] Nodes { get; private set; }
            public double GeometricError { get; private set; }
        }
    }
}