using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CjClutter.OpenGl.Camera;
using CjClutter.OpenGl.EntityComponent;
using CjClutter.OpenGl.Gui;
using CjClutter.OpenGl.Noise;
using CjClutter.OpenGl.OpenGl;
using CjClutter.OpenGl.OpenGl.Shaders;
using CjClutter.OpenGl.OpenTk;
using CjClutter.OpenGl.SceneGraph;
using NUnit.Framework.Constraints;
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
        }

        private static ChunkedLodTreeFactory.ChunkedLodTreeNode CreateTree()
        {
            var chunkedLodTreeFactory = new ChunkedLodTreeFactory();

            var bounds = new Box3D(
                new Vector3d(-4096, -4096, 0),
                new Vector3d(4096, 4096, 0));

            var levels = (int)Math.Log(4096 / 10, 2);
            //calculate depth so that one square is one meter as maximum resolution
            return chunkedLodTreeFactory.Create(bounds, 9);
        }

        public void Render(ICamera camera)
        {
            var visibleChunks = _chunkedLod.Calculate(
                _tree,
                camera.Width,
                camera.HorizontalFieldOfView,
                camera.Position.Xzy,
                50);

            Console.WriteLine(camera.Position);

            GL.ClearColor(Color4.White);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.Enable(EnableCap.DepthTest);
            GL.CullFace(CullFaceMode.Back);
            GL.Enable(EnableCap.CullFace);
            GL.FrontFace(FrontFaceDirection.Cw);

            _simpleMaterial.Bind();
            var terrainChunkFactory = new TerrainChunkFactory();
            var resourceAllocator = new ResourceAllocator(new OpenGlResourceFactory());

            foreach (var chunkedLodTreeNode in visibleChunks)
            {
                if (!_cache.ContainsKey(chunkedLodTreeNode))
                {
                    var mesh = terrainChunkFactory.Create(chunkedLodTreeNode.Bounds);
                    _count = mesh.Faces.Length * 3;
                    var renderable = resourceAllocator.AllocateResourceFor(mesh);
                    _cache.Add(chunkedLodTreeNode, renderable);
                }

                var renderableMesh = _cache[chunkedLodTreeNode];

                _simpleMaterial.LightDirection.Set(new Vector3(0, 1, 0));
                _simpleMaterial.ProjectionMatrix.Set(camera.ComputeProjectionMatrix().ToMatrix4());
                _simpleMaterial.ViewMatrix.Set(camera.ComputeCameraMatrix().ToMatrix4());

                renderableMesh.VertexArrayObject.Bind();

                var bounds = chunkedLodTreeNode.Bounds;
                var translation = Matrix4.CreateTranslation((float)bounds.Center.X, 0, (float)bounds.Center.Y);
                var delta = bounds.Max - bounds.Min;
                var scale = Matrix4.CreateScale((float)delta.X, 1, (float)delta.Y);

                _simpleMaterial.ModelMatrix.Set(scale * translation);
                _simpleMaterial.Color.Set(new Vector4(0, 0, 1.0f, 0));

                //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                //GL.Disable(EnableCap.CullFace);

                GL.DrawElements(BeginMode.Triangles, _count, DrawElementsType.UnsignedInt, 0);

                //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                //GL.Enable(EnableCap.CullFace);

                renderableMesh.VertexArrayObject.Unbind();
            }
            _simpleMaterial.Unbind();
        }

        private readonly Dictionary<ChunkedLodTreeFactory.ChunkedLodTreeNode, RenderableMesh> _cache = new Dictionary<ChunkedLodTreeFactory.ChunkedLodTreeNode, RenderableMesh>();
        private SimpleMaterial _simpleMaterial;
        private int _count;
    }

    public class TerrainChunkFactory
    {
        public Mesh3V3N Create(Box3D bounds)
        {
            var meshDimensions = 32;
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
                return _noise.Noise(x / 30, y / 30) * 3;
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
                var right = new Vector3d(center.X + 1, center.Y, _noiseGenerator.Noise(center.X + 1, center.Y));
                var left = new Vector3d(center.X - 1, center.Y, _noiseGenerator.Noise(center.X - 1, center.Y));
                var top = new Vector3d(center.X, center.Y + 1, _noiseGenerator.Noise(center.X, center.Y + 1));
                var bottom = new Vector3d(center.X, center.Y - 1, _noiseGenerator.Noise(center.X, center.Y - 1));

                return Vector3d.Cross(right - left, top - bottom).Normalized();
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

