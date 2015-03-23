using System.Linq;
using CjClutter.OpenGl;
using CjClutter.OpenGl.EntityComponent;
using NUnit.Framework;
using OpenTK;

namespace ObjLoader.Test
{
    public class ChunkedLodTreeFactoryTests
    {
        [Test]
        public void Creates_root_node_with_no_leafs_at_zero_depth()
        {
            var sut = new ChunkedLodTreeFactory();
            var expectedBounds = new Box3D(new Vector3d(-1, -2, -3), new Vector3d(1, 2, 3));

            var result = sut.Create(expectedBounds, 0);

            Assert.AreEqual(expectedBounds, result.Bounds);
            Assert.AreEqual(1, result.GeometricError);
            Assert.IsEmpty(result.Nodes);
            Assert.IsNull(result.Parent);
        }

        [Test]
        public void Creates_root_node_with_four_children_at_depth_one()
        {
            var sut = new ChunkedLodTreeFactory();
            var expectedBounds = new Box3D(new Vector3d(-1, -1, -1), new Vector3d(1, 1, 1));

            var result = sut.Create(expectedBounds, 1);

            Assert.AreEqual(expectedBounds, result.Bounds);
            Assert.AreEqual(2, result.GeometricError);
            Assert.AreEqual(4, result.Nodes.Length);
        }

        [Test]
        public void Creates_children_with_correct_bounds_and_are_leafs_at_depth_one()
        {
            var sut = new ChunkedLodTreeFactory();
            var expectedBounds = new Box3D(new Vector3d(-1, -1, -1), new Vector3d(1, 1, 1));

            var result = sut.Create(expectedBounds, 1);

            AssertLeaf(new Box3D(new Vector3d(-1, -1, -1), new Vector3d(0, 0, 1)), result.Nodes[0]);
            AssertLeaf(new Box3D(new Vector3d(0, -1, -1), new Vector3d(1, 0, 1)), result.Nodes[1]);
            AssertLeaf(new Box3D(new Vector3d(-1, 0, -1), new Vector3d(0, 1, 1)), result.Nodes[2]);
            AssertLeaf(new Box3D(new Vector3d(0, 0, -1), new Vector3d(1, 1, 1)), result.Nodes[3]);
        }

        [Test]
        public void Creates_tree_for_depth_of_two()
        {
            var sut = new ChunkedLodTreeFactory();
            var expectedBounds = new Box3D(new Vector3d(-1, -1, -1), new Vector3d(1, 1, 1));

            var result = sut.Create(expectedBounds, 2);
            Assert.AreEqual(4, result.GeometricError);
            var leafs = result.Nodes.SelectMany(x => x.Nodes);

            Assert.AreEqual(16, leafs.Count());
            foreach (var leaf in leafs)
            {
                Assert.IsEmpty(leaf.Nodes);
            }
        }

        private void AssertLeaf(Box3D expected, ChunkedLodTreeFactory.ChunkedLodTreeNode actual)
        {
            Assert.IsEmpty(actual.Nodes);
            Assert.AreEqual(1, actual.GeometricError);
            Assert.AreEqual(expected, actual.Bounds);
        }
    }
}