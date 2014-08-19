using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
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
    public class TerrainChunkCache
    {
        private readonly Dictionary<ChunkedLodTreeFactory.ChunkedLodTreeNode, RenderableMesh> _cache = new Dictionary<ChunkedLodTreeFactory.ChunkedLodTreeNode, RenderableMesh>();
        private TerrainChunkFactory _terrainChunkFactory;
        private BlockingCollection<Box3D> _queue;

        public TerrainChunkCache(TerrainChunkFactory terrainChunkFactory)
        {
            _terrainChunkFactory = terrainChunkFactory;
            _queue = new BlockingCollection<Box3D>();
            var workerThread = new Thread(() => Worker(_queue));
        }

        private void Worker(BlockingCollection<Box3D> queue)
        {
            while (true)
            {
                var bounds = queue.Take();
                var mesh = _terrainChunkFactory.Create(bounds);
            }
        }

        public void LoadIfNotCachedAsync(Box3D bounds)
        {

        }
    }

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
            _cache = new TerrainChunkCache(new TerrainChunkFactory());
        }

        private static ChunkedLodTreeFactory.ChunkedLodTreeNode CreateTree()
        {
            var chunkedLodTreeFactory = new ChunkedLodTreeFactory();

            var bounds = new Box3D(
                new Vector3d(-4096, -4096, 0),
                new Vector3d(4096, 4096, 0));

            var levels = Math.Log((8192 / 128), 2);

            //calculate depth so that one square is one meter as maximum resolution
            return chunkedLodTreeFactory.Create(bounds, (int)levels);
        }

        public void Render(ICamera camera, Vector3d lightPosition)
        {
            var transformation = new Matrix4d(
                new Vector4d(1, 0, 0, 0),
                new Vector4d(0, 0, 1, 0),
                new Vector4d(0, 1, 0, 1),
                new Vector4d(0, 0, 0, 1));

            var visibleChunks = _chunkedLod.Calculate(
                _tree,
                camera.Width,
                camera.HorizontalFieldOfView,
                Vector3d.Transform(camera.Position, transformation),
                50,
                FrustumPlaneExtractor.ExtractRowMajor(transformation * camera.ComputeCameraMatrix() * camera.ComputeProjectionMatrix()));

            var terrainChunkFactory = new TerrainChunkFactory();
            var resourceAllocator = new ResourceAllocator(new OpenGlResourceFactory());

            foreach (var chunkedLodTreeNode in visibleChunks)
            {
                _cache.LoadIfNotCachedAsync(chunkedLodTreeNode.Bounds);
                //if (!_cache.ContainsKey(chunkedLodTreeNode))
                //{
                //    var mesh = terrainChunkFactory.Create(chunkedLodTreeNode.Bounds);
                //    _count = mesh.Faces.Length * 3;
                //    var renderable = resourceAllocator.AllocateResourceFor(mesh);
                //    _cache.Add(chunkedLodTreeNode, renderable);
                //}
            }

            GL.ClearColor(Color4.White);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.Enable(EnableCap.DepthTest);
            GL.CullFace(CullFaceMode.Back);
            GL.Enable(EnableCap.CullFace);
            GL.FrontFace(FrontFaceDirection.Cw);

            _simpleMaterial.Bind();
            var worldSpaceLightPosition = new Vector4d(lightPosition, 1);
            //_simpleMaterial.LightDirection.Set(worldSpaceLightPosition);

            _simpleMaterial.ProjectionMatrix.Set(camera.ComputeProjectionMatrix().ToMatrix4());
            _simpleMaterial.ViewMatrix.Set(camera.ComputeCameraMatrix().ToMatrix4());

            _simpleMaterial.Color.Set(new Vector4(0.9f, 0.9f, 0.9f, 1.0f));

            foreach (var chunkedLodTreeNode in visibleChunks)
            {
                if (!_cache.ContainsKey(chunkedLodTreeNode))
                {
                    continue;
                }
                var renderableMesh = _cache[chunkedLodTreeNode];
                renderableMesh.VertexArrayObject.Bind();

                var bounds = chunkedLodTreeNode.Bounds;
                var translation = Matrix4.CreateTranslation((float)bounds.Center.X, 0, (float)bounds.Center.Y);
                var delta = bounds.Max - bounds.Min;
                var scale = Matrix4.CreateScale((float)delta.X, 10f, (float)delta.Y);

                var modelMatrix = scale * translation;
                _simpleMaterial.ModelMatrix.Set(modelMatrix);

                var worldToModel = modelMatrix.Inverted();
                var transform = Vector4.Transform((Vector4)worldSpaceLightPosition, worldToModel);
                _simpleMaterial.LightDirection.Set(transform.Xyz);

                //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                //GL.Disable(EnableCap.CullFace);

                GL.DrawElements(BeginMode.Triangles, _count, DrawElementsType.UnsignedInt, 0);

                //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                //GL.Enable(EnableCap.CullFace);

                //_normalDebugProgram.Bind();
                //_normalDebugProgram.ProjectionMatrix.Set(camera.ComputeProjectionMatrix().ToMatrix4());
                //_normalDebugProgram.ViewMatrix.Set(camera.ComputeCameraMatrix().ToMatrix4());
                //_normalDebugProgram.ModelMatrix.Set(scale * translation);
                //GL.DrawElements(BeginMode.Triangles, _count, DrawElementsType.UnsignedInt, 0);
                //_normalDebugProgram.Unbind();

                renderableMesh.VertexArrayObject.Unbind();
            }
            _simpleMaterial.Unbind();

            //_normalDebugProgram.Bind();
            //_normalDebugProgram.ProjectionMatrix.Set(camera.ComputeProjectionMatrix().ToMatrix4());
            //_normalDebugProgram.ViewMatrix.Set(camera.ComputeCameraMatrix().ToMatrix4());

            //foreach (var chunkedLodTreeNode in visibleChunks)
            //{
            //    var renderableMesh = _cache[chunkedLodTreeNode];
            //    renderableMesh.VertexArrayObject.Bind();

            //    var bounds = chunkedLodTreeNode.Bounds;
            //    var translation = Matrix4.CreateTranslation((float)bounds.Center.X, 0, (float)bounds.Center.Y);
            //    var delta = bounds.Max - bounds.Min;
            //    var scale = Matrix4.CreateScale((float)delta.X, 1, (float)delta.Y);

            //    _normalDebugProgram.ModelMatrix.Set(scale * translation);

            //    GL.DrawElements(BeginMode.Triangles, _count, DrawElementsType.UnsignedInt, 0);

            //    renderableMesh.VertexArrayObject.Unbind();
            //}

            //_normalDebugProgram.Unbind();
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
            return MeshCreator.CreateFromHeightMap(meshDimensions, meshDimensions, implicintHeightMap);
        }

        public class ScaledNoiseGenerator : INoiseGenerator
        {
            private INoiseGenerator _noise;

            public ScaledNoiseGenerator()
            {
                _noise = new NoiseFactory.RidgedMultiFractal().Create(new NoiseFactory.NoiseParameters());
            }

            public double Noise(double x, double y)
            {
                return _noise.Noise(x / 100, y / 100);
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

                return -Vector3d.Cross((right - left).Normalized(), (top - bottom).Normalized()).Normalized();
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

