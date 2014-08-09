using System;
using CjClutter.OpenGl.Camera;
using CjClutter.OpenGl.Noise;
using CjClutter.OpenGl.OpenGl.VertexTypes;
using OpenTK;

namespace CjClutter.OpenGl.EntityComponent
{
    public class ChunkedLODSystem : IEntitySystem
    {
        private readonly ICamera _camera;
        private Node _root;
        private Vector4d[] _frustumPlanes;

        public ChunkedLODSystem(ICamera camera)
        {
            _camera = camera;
        }

        private Node CreateNode(Box3D bounds, int level, EntityManager entityManager)
        {
            var mesh = GridCreator.CreateXZ(10, 10);
            var staticMesh = new StaticMesh
            {
                Color = new Vector4(0f, 0f, 1f, 1f),
                ModelMatrix = Matrix4.Identity,
            };

            var size = bounds.Max - bounds.Min;
            var mesh3V3N = mesh.Transformed(Matrix4.CreateScale((float)size.X, 1, (float)size.Z) * Matrix4.CreateTranslation((Vector3)bounds.Center));
            var improvedPerlinNoise = new ImprovedPerlinNoise(4711);
            for (int i = 0; i < mesh3V3N.Vertices.Length; i++)
            {
                var vertex = mesh3V3N.Vertices[i];
                var height = improvedPerlinNoise.Noise(vertex.Position.X, vertex.Position.Z) * 0.2;
                mesh3V3N.Vertices[i] = new Vertex3V3N
                {
                    Normal = new Vector3(0, 1, 0),
                    Position = new Vector3(vertex.Position.X, (float)height, vertex.Position.Z)
                };
            }

            staticMesh.Update(mesh3V3N);

            var entity = new Entity(Guid.NewGuid().ToString());
            entityManager.Add(entity);
            entityManager.AddComponentToEntity(entity, staticMesh);

            if (level == 0)
            {
                return new Node(bounds, new Node[] { }, entity, 1);
            }

            var min = bounds.Min;
            var max = bounds.Max;
            var center = bounds.Center;

            return new Node(bounds,
                new[]
                {
                    CreateNode(new Box3D(bounds.Min, center), level -1, entityManager),
                    CreateNode(new Box3D(new Vector3d(center.X, 0, min.Z), new Vector3d(max.X, 0, center.Z)), level -1, entityManager),
                    CreateNode(new Box3D(new Vector3d(min.X, 0, center.Z), new Vector3d(center.X, 0, max.Z)), level - 1, entityManager),
                    CreateNode(new Box3D(center, max), level - 1, entityManager)
                }, entity, Math.Pow(2, level));
        }

        public void Update(double elapsedTime, EntityManager entityManager)
        {
            if (_root == null)
            {
                _root = CreateNode(new Box3D(new Vector3d(-100, 0, -100f), new Vector3d(100f, 0, 100f)), 6, entityManager);
            }

            _frustumPlanes = FrustumPlaneExtractor.ExtractRowMajor(_camera);
            var k = _camera.Width / (Math.Tan(_camera.HorizontalFieldOfView / 2));
            ComputeLod(_root, k, entityManager);
        }

        private void ComputeLod(Node root, double k, EntityManager entityManager)
        {
            var mesh = entityManager.GetComponent<StaticMesh>(root.Entity);

            var side = (root.Bounds.Max - root.Bounds.Min).X;
            var radius = Math.Sqrt(side*side + side*side);

            for (int i = 0; i < 6; i++)
            {
                if (PlaneDistance(_frustumPlanes[i], root.Bounds.Center) <= -radius)
                {
                    return;
                }
            }

            var error = root.GeometricError;
            var distance = (root.Bounds.Center - _camera.Position).Length;
            var rho = (error / distance) * k;

            var threshhold = 100;
            if (rho <= threshhold || root.Leafs.Length == 0)
            {
                mesh.IsVisible = true;
            }
            else
            {
                for (int j = 0; j < root.Leafs.Length; j++)
                {
                    ComputeLod(root.Leafs[j], k, entityManager);
                }
            }
        }

        private double PlaneDistance(Vector4d plane, Vector3d pt)
        {
            return plane.X * pt.X + plane.Y * pt.Y + plane.Z * pt.Z + plane.W;
        }
    }

    public class Node
    {
        public Node(Box3D bounds, Node[] leafNodes, Entity entity, double geometricError)
        {
            Entity = entity;
            GeometricError = geometricError;
            Bounds = bounds;
            Leafs = leafNodes;
        }

        public Entity Entity { get; private set; }
        public double GeometricError { get; private set; }
        public Box3D Bounds { get; private set; }
        public Node[] Leafs { get; private set; }
    }
}