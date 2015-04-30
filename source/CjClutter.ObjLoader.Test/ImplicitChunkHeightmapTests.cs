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

            var sut = new TerrainChunkFactory.ImplicitChunkHeightMap(new Bounds2D(new Vector2d(-5, 5), new Vector2d(5, 10)), 10, 20, _noiseGenerator);
            var result = sut.GetHeight(3, 12);

            Assert.AreEqual(47, result);
        }

        [Test]
        public void Get_normal_scales_row_and_column_according_to_bounds_when_accessing_noise()
        {
            _noiseGenerator.Stub(x => x.Noise(-3, 8)).Return(40); 
            _noiseGenerator.Stub(x => x.Noise(-1, 8)).Return(42);
            _noiseGenerator.Stub(x => x.Noise(-2, 7)).Return(10);
            _noiseGenerator.Stub(x => x.Noise(-2, 9)).Return(15);

            var sut = new TerrainChunkFactory.ImplicitChunkHeightMap(new Bounds2D(new Vector2d(-5, 5), new Vector2d(5, 10)), 10, 20, _noiseGenerator);
            var result = sut.GetNormal(3, 12);


            Assert.AreEqual(new Vector3d(-4, -10, 4).Normalized(), result);
        }
    }
}