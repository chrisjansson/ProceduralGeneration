using System;
using System.Collections.Generic;
using OpenTK;

namespace CjClutter.OpenGl
{
    public class ChunkedLod
    {
        private List<ChunkedLodTreeFactory.ChunkedLodTreeNode> _visibleNodes;
        private Vector3d _cameraPosition;
        private double _k;
        private double _allowedScreenSpaceError;

        public List<ChunkedLodTreeFactory.ChunkedLodTreeNode> Calculate(
            ChunkedLodTreeFactory.ChunkedLodTreeNode root, 
            double viewportWidth, 
            double horizontalFieldOfView, 
            Vector3d cameraPosition, 
            double allowedScreenSpaceError)
        {
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
            var distanceToCamera = (node.Bounds.Center - _cameraPosition).Length;
            var screenSpaceError = (node.GeometricError/distanceToCamera)*_k;

            return screenSpaceError <= _allowedScreenSpaceError;
        }

        private bool IsVisible(ChunkedLodTreeFactory.ChunkedLodTreeNode node)
        {
            return true;
        }
    }
}