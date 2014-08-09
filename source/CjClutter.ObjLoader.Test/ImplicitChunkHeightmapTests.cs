using CjClutter.OpenGl;
using CjClutter.OpenGl.EntityComponent;
using CjClutter.OpenGl.Noise;
using NUnit.Framework;
using OpenTK;
using Rhino.Mocks;

namespace ObjLoader.Test
{
    public class ImplicitChunkHeightMapTests
    {
        private INoiseGenerator _noiseGenerator;

        [SetUp]
        public void SetUp()
        {
            _noiseGenerator = MockRepository.GenerateStub<INoiseGenerator>();
        }

        [Test]
        public void Get_height_scales_row_and_column_according_to_bounds_when_accessing_noise()
        {
            _noiseGenerator.Stub(x => x.Noise(-2, 8)).Return(47);

            var sut = new TerrainChunkFactory.ImplicitChunkHeightMap(new Box3D(new Vector3d(-5, 5, 0), new Vector3d(5, 10, 0)), 10, 20, _noiseGenerator);
            var result = sut.GetHeight(3, 12);

            Assert.AreEqual(47, result);
        }
    }
}