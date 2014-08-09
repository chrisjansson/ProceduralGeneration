namespace CjClutter.OpenGl
{
    public static class ChunkedLodTreeNodeExtensions
    {
        public static bool IsLeaf(this ChunkedLodTreeFactory.ChunkedLodTreeNode node)
        {
            return node.Nodes.Length == 0;
        }
    }
}