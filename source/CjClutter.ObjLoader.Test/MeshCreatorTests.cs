using CjClutter.OpenGl.EntityComponent;
using FluentAssertions;
using NUnit.Framework;
using OpenTK;

namespace ObjLoader.Test
{
    [TestFixture]
    public class MeshCreatorTests
    {
        [Test]
        public void Mesh_has_correct_size()
        {
            var mesh = MeshCreator.CreateXZGrid(10, 10);

            Assert.AreEqual(11 * 11, mesh.Vertices.Length);
            Assert.AreEqual(10 * 10 * 2, mesh.Faces.Length);
        }

        [Test]
        public void Vertices_has_correct_positions()
        {
            var mesh = MeshCreator.CreateXZGrid(1, 1);

            Assert.AreEqual(new Vector3(-0.5f, 0, -0.5f), mesh.Vertices[0].Position);
            Assert.AreEqual(new Vector3(-0.5f, 0, 0.5f), mesh.Vertices[1].Position);
            Assert.AreEqual(new Vector3(0.5f, 0, -0.5f), mesh.Vertices[2].Position);
            Assert.AreEqual(new Vector3(0.5f, 0, 0.5f), mesh.Vertices[3].Position);
        }
    }
}