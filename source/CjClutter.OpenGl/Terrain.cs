using System;
using CjClutter.OpenGl.Camera;
using CjClutter.OpenGl.EntityComponent;
using CjClutter.OpenGl.Gui;
using CjClutter.OpenGl.Noise;
using CjClutter.OpenGl.OpenGl;
using CjClutter.OpenGl.OpenGl.Shaders;
using CjClutter.OpenGl.OpenTk;
using CjClutter.OpenGl.SceneGraph;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace CjClutter.OpenGl
{
    public class Terrain
    {
        private readonly ChunkedLodTreeFactory.ChunkedLodTreeNode _tree;
        private readonly ChunkedLod _chunkedLod;

        public Terrain()
        {
            _tree = CreateTree();
            _chunkedLod = new ChunkedLod();
            _simpleMaterial = new SimpleMaterial();
            _simpleMaterial.Create();
            _normalDebugProgram = new NormalDebugProgram();
            _normalDebugProgram.Create();
            _cache = new TerrainChunkCache(new TerrainChunkFactory(), new ResourceAllocator(new OpenGlResourceFactory()));
        }

        private static ChunkedLodTreeFactory.ChunkedLodTreeNode CreateTree()
        {
            var chunkedLodTreeFactory = new ChunkedLodTreeFactory();

            var bounds = new Box3D(
                new Vector3d(-4096, -4096, 0),
                new Vector3d(4096, 4096, 0));

            var levels = Math.Log((8192 / 128), 2);

            //calculate depth so that one square is one meter as maximum resolution
            return chunkedLodTreeFactory.Create(bounds, (int)8);
        }

        public void Render(ICamera camera, ICamera lodCamera, Vector3d lightPosition)
        {
            var transformation = new Matrix4d(
                new Vector4d(1, 0, 0, 0),
                new Vector4d(0, 0, 1, 0),
                new Vector4d(0, 1, 0, 1),
                new Vector4d(0, 0, 0, 1));

            var visibleChunks = _chunkedLod.Calculate(
                _tree,
                lodCamera.Width,
                lodCamera.HorizontalFieldOfView,
                Vector3d.Transform(lodCamera.Position, transformation),
                20,
                FrustumPlaneExtractor.ExtractRowMajor(transformation * lodCamera.ComputeCameraMatrix() * lodCamera.ComputeProjectionMatrix()));

            GL.ClearColor(Color4.White);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.Enable(EnableCap.DepthTest);
            GL.CullFace(CullFaceMode.Back);
            GL.Enable(EnableCap.CullFace);
            GL.FrontFace(FrontFaceDirection.Cw);

            _simpleMaterial.Bind();

            _simpleMaterial.ProjectionMatrix.Set(camera.ComputeProjectionMatrix().ToMatrix4());
            _simpleMaterial.ViewMatrix.Set(camera.ComputeCameraMatrix().ToMatrix4());
            _simpleMaterial.LightDirection.Set(new Vector3(10, 2, 10).Normalized());

            _simpleMaterial.Color.Set(new Vector4(0.9f, 0.9f, 0.9f, 1.0f));

            foreach (var chunkedLodTreeNode in visibleChunks)
            {
                var renderableMesh = _cache.GetRenderable(chunkedLodTreeNode.Bounds);
                if (renderableMesh == null)
                    continue;

                renderableMesh.CreateVAO();
                renderableMesh.VertexArrayObject.Bind();

                var bounds = chunkedLodTreeNode.Bounds;
                var translation = Matrix4.CreateTranslation((float)bounds.Center.X, 0, (float)bounds.Center.Y);
                var delta = bounds.Max - bounds.Min;
                var scale = Matrix4.CreateScale((float)delta.X, 1, (float)delta.Y);

                var modelMatrix = scale * translation;
                _simpleMaterial.ModelMatrix.Set(modelMatrix);

                var normalToWorld = modelMatrix.Inverted();
                var transposed = Matrix4.Transpose(normalToWorld);
                var normalToWorld3x3 = new Matrix3(transposed);
                _simpleMaterial.NormalToWorld3x3.Set(normalToWorld3x3);

                _simpleMaterial.LightPosition.Set((Vector3)lightPosition);

                GL.DrawElements(BeginMode.Triangles, renderableMesh.Faces * 3, DrawElementsType.UnsignedInt, 0);

                renderableMesh.VertexArrayObject.Unbind();
            }
            _simpleMaterial.Unbind();
        }

        private SimpleMaterial _simpleMaterial;
        private int _count;
        private NormalDebugProgram _normalDebugProgram;
        private TerrainChunkCache _cache;
    }

    public class TerrainChunkFactory
    {
        public Mesh3V3N Create(Box3D bounds)
        {
            var meshDimensions = 128;
            var implicintHeightMap = new ImplicitChunkHeightMap(bounds, meshDimensions, meshDimensions, new ScaledNoiseGenerator());
            return MeshCreator.CreateFromHeightMap(meshDimensions, meshDimensions, implicintHeightMap);//.Transformed(Matrix4.CreateScale(256f, 1f, 256f));
        }

        public class ScaledNoiseGenerator : INoiseGenerator
        {
            private INoiseGenerator _noise;
            private ImprovedPerlinNoise _largeScaleNoise;

            public ScaledNoiseGenerator()
            {
                _largeScaleNoise = new ImprovedPerlinNoise(4711);
                _noise = new NoiseFactory.RidgedMultiFractal().Create(new NoiseFactory.NoiseParameters());
            }

            public double Noise(double x, double y)
            {
                return _noise.Noise(x / 200, y / 200) * 30;
            }

            public double Noise(double x, double y, double z)
            {
                throw new NotImplementedException();
            }
        }

        public class ImplicitChunkHeightMap : IHeightMap
        {
            private Box3D _bounds;
            private readonly INoiseGenerator _noiseGenerator;
            private readonly int _columns;
            private readonly int _rows;

            public ImplicitChunkHeightMap(Box3D bounds, int columns, int rows, INoiseGenerator noiseGenerator)
            {
                _noiseGenerator = noiseGenerator;
                _rows = rows;
                _columns = columns;
                _bounds = bounds;
            }

            public double GetHeight(int column, int row)
            {
                var position = CalculatePosition(column, row);
                return _noiseGenerator.Noise(position.X, position.Y);
            }

            public Vector3d GetNormal(int column, int row)
            {
                var center = CalculatePosition(column, row);
                var right = new Vector3d(center.X + 1, _noiseGenerator.Noise(center.X + 1, center.Y), center.Y);
                var left = new Vector3d(center.X - 1, _noiseGenerator.Noise(center.X - 1, center.Y), center.Y);
                var top = new Vector3d(center.X, _noiseGenerator.Noise(center.X, center.Y + 1), center.Y + 1);
                var bottom = new Vector3d(center.X, _noiseGenerator.Noise(center.X, center.Y - 1), center.Y - 1);

                return -(Vector3d.Cross((right - left).Normalized(), (top - bottom).Normalized()).Normalized());
            }

            private Vector2d CalculatePosition(int column, int row)
            {
                var delta = _bounds.Max - _bounds.Min;

                var columnFraction = column / (double)_columns;
                var rowFraction = row / (double)_rows;

                var x = _bounds.Min.X + delta.X * columnFraction;
                var y = _bounds.Min.Y + delta.Y * rowFraction;

                return new Vector2d(x, y);
            }
        }
    }
}

