using System;
using System.Collections.Generic;
using OpenTK;

namespace CjClutter.OpenGl
{
    public class ChunkedLod : IChunkedLod
    {
        private List<ChunkedLodTreeFactory.ChunkedLodTreeNode> _visibleNodes;
        private Vector3d _cameraPosition;
        private double _k;
        private double _allowedScreenSpaceError;
        private Vector4d[] _frustumPlanes;

        public List<ChunkedLodTreeFactory.ChunkedLodTreeNode> Calculate(
            ChunkedLodTreeFactory.ChunkedLodTreeNode root,
            double viewportWidth,
            double horizontalFieldOfView,
            Vector3d cameraPosition,
            double allowedScreenSpaceError,
            Vector4d[] frustumPlanes)
        {
            _frustumPlanes = frustumPlanes;
            _allowedScreenSpaceError = allowedScreenSpaceError;
            _cameraPosition = cameraPosition;
            _visibleNodes = new List<ChunkedLodTreeFactory.ChunkedLodTreeNode>();
            _k = viewportWidth / (Math.Tan(horizontalFieldOfView / 2));

            CalculateVisibleNodes(root);

            return _visibleNodes;
        }

        private void CalculateVisibleNodes(ChunkedLodTreeFactory.ChunkedLodTreeNode node)
        {
            if (!IsVisible(node))
                return;

            if (IsDetailedEnough(node) || node.IsLeaf())
            {
                _visibleNodes.Add(node);
                return;
            }

            foreach (var child in node.Nodes)
            {
                CalculateVisibleNodes(child);
            }
        }

        private bool IsDetailedEnough(ChunkedLodTreeFactory.ChunkedLodTreeNode node)
        {
            var nodeCenter = new Vector3d(node.Bounds.Center.X, node.Bounds.Center.Y, 0);
            var distanceToCamera = (nodeCenter - _cameraPosition).Length;
            var screenSpaceError = (node.GeometricError / distanceToCamera) * _k;

            return screenSpaceError <= _allowedScreenSpaceError;
        }

        private bool IsVisible(ChunkedLodTreeFactory.ChunkedLodTreeNode node)
        {
            var center = new Vector3d(node.Bounds.Center.X, node.Bounds.Center.Y, 0);
            var delta = node.Bounds.Max - node.Bounds.Min;
            var side = Math.Max(delta.X, delta.Y);
            var radius = Math.Sqrt(side * side + side * side);

            for (var i = 0; i < _frustumPlanes.Length; i++)
            {
                var sphereIsOutsideOfPlane = PlaneDistance(_frustumPlanes[i], center) < -radius;
                if (sphereIsOutsideOfPlane)
                {
                    return false;
                }
            }

            return true;
        }

        private double PlaneDistance(Vector4d plane, Vector3d pt)
        {
            return plane.X * pt.X + plane.Y * pt.Y + plane.Z * pt.Z + plane.W;
        }
    }
}