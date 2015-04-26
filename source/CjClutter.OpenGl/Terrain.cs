using System;
using System.Collections.Generic;
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
    public interface IChunkedLod
    {
        List<ChunkedLodTreeFactory.ChunkedLodTreeNode> Calculate(
            ChunkedLodTreeFactory.ChunkedLodTreeNode root,
            double viewportWidth,
            double horizontalFieldOfView,
            Vector3d cameraPosition,
            double allowedScreenSpaceError,
            Vector4d[] frustumPlanes);
    }

    public class Terrain
    {
        private readonly ChunkedLodTreeFactory.ChunkedLodTreeNode _tree;
        private readonly IChunkedLod _chunkedLod;

        public Terrain(IChunkedLod chunkedLod)
        {
            _tree = CreateTree();
            _chunkedLod = chunkedLod;
            _simpleMaterial = new SimpleMaterial();
            _simpleMaterial.Create();
            _normalDebugProgram = new NormalDebugProgram();
            _normalDebugProgram.Create();
            _cache = new TerrainChunkCache(new TerrainChunkFactory(), new ResourceAllocator(new OpenGlResourceFactory()));
        }

        public static ChunkedLodTreeFactory.ChunkedLodTreeNode CreateTree()
        {
            var chunkedLodTreeFactory = new ChunkedLodTreeFactory();

            double width = 8192 * 2 * 2;
            var bounds = new Bounds2D(
                new Vector2d(-width/2, -width/2),
                new Vector2d(width/2, width/2));

            var chunkResolution = 32;
            var depth = (int)Math.Log((width / chunkResolution), 2);

            return chunkedLodTreeFactory.Create(bounds, depth);
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
                30,
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
            _simpleMaterial.LightDirection.Set(new Vector3(1, 1, 0).Normalized());

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
                _simpleMaterial.NormalToWorld3x3.Set(Matrix3.Identity);

                _simpleMaterial.LightPosition.Set((Vector3)lightPosition);

                GL.DrawElements(BeginMode.Triangles, renderableMesh.Faces * 3, DrawElementsType.UnsignedInt, 0);

                renderableMesh.VertexArrayObject.Unbind();
            }
            _simpleMaterial.Unbind();
        }

        private SimpleMaterial _simpleMaterial;
        private NormalDebugProgram _normalDebugProgram;
        private TerrainChunkCache _cache;
    }

    public class TerrainChunkFactory
    {
        public Mesh3V3N Create(Bounds2D bounds)
        {
            var meshDimensions = 128;
            var implicintHeightMap = new ImplicitChunkHeightMap(bounds, meshDimensions, meshDimensions, new ScaledNoiseGenerator());
            return MeshCreator.CreateFromHeightMap(meshDimensions, meshDimensions, implicintHeightMap);
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
                return _noise.Noise(x / 400, y / 400) * 30;
            }

            public double Noise(double x, double y, double z)
            {
                throw new NotImplementedException();
            }
        }

        public class ImplicitChunkHeightMap : IHeightMap
        {
            private Bounds2D _bounds;
            private readonly INoiseGenerator _noiseGenerator;
            private readonly int _columns;
            private readonly int _rows;
            private double _dx;
            private double _dy;

            public ImplicitChunkHeightMap(Bounds2D bounds, int columns, int rows, INoiseGenerator noiseGenerator)
            {
                _noiseGenerator = noiseGenerator;
                _rows = rows;
                _columns = columns;
                _bounds = bounds;

                var origin = CalculatePosition(0, 0);
                var position = CalculatePosition(1, 1);

                _dx = 1 / (position.X - origin.X);
                _dy = 1 / (position.Y - origin.Y);
            }

            public double GetHeight(int column, int row)
            {
                var position = CalculatePosition(column, row);
                return _noiseGenerator.Noise(position.X, position.Y);
            }

            public Vector3d GetNormal(int column, int row)
            {
                var center = CalculatePosition(column, row);
                double d = 1;
                var leftRight = new Vector3d(d * 2, _noiseGenerator.Noise(center.X + d, center.Y) - _noiseGenerator.Noise(center.X - d, center.Y), 0);
                var bottomTop = new Vector3d(0, _noiseGenerator.Noise(center.X, center.Y - d) - _noiseGenerator.Noise(center.X, center.Y + d), d * 2);

                var normal = -(Vector3d.Cross(leftRight.Normalized(), bottomTop.Normalized()).Normalized());
                return normal;
            }

            private Vector2d CalculatePosition(double column, double row)
            {
                var delta = _bounds.Max - _bounds.Min;

                var columnFraction = column / _columns;
                var rowFraction = row / _rows;

                var x = _bounds.Min.X + delta.X * columnFraction;
                var y = _bounds.Min.Y + delta.Y * rowFraction;

                return new Vector2d(x, y);
            }
        }
    }
}

