using System;
using CjClutter.OpenGl.Camera;
using CjClutter.OpenGl.Noise;
using CjClutter.OpenGl.OpenGl.VertexTypes;
using CjClutter.OpenGl.SceneGraph;
using OpenTK;

namespace CjClutter.OpenGl.EntityComponent
{
    public class ChunkedLODSystem : IEntitySystem
    {
        public ChunkedLODSystem(ICamera camera)
        {
            _camera = camera;
        }

        private Node CreateNode(Box3d bounds, int level, EntityManager entityManager)
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
                var height = improvedPerlinNoise.Noise(vertex.Position.X, vertex.Position.Z)  * 0.2;
                mesh3V3N.Vertices[i] = new Vertex3V3N
                {
                    Normal = new Vector3(0, 1, 0),
                    Position = new Vector3(vertex.Position.X, (float) height, vertex.Position.Z)
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
                    CreateNode(new Box3d(bounds.Min, center), level -1, entityManager),
                    CreateNode(new Box3d(new Vector3d(center.X, 0, min.Z), new Vector3d(max.X, 0, center.Z)), level -1, entityManager),
                    CreateNode(new Box3d(new Vector3d(min.X, 0, center.Z), new Vector3d(center.X, 0, max.Z)), level - 1, entityManager),
                    CreateNode(new Box3d(center, max), level - 1, entityManager)
                }, entity, Math.Pow(2, level));
        }

        private bool _first = true;
        private Node _root;
        private ICamera _camera;

        public void Update(double elapsedTime, EntityManager entityManager)
        {
            if (_first)
            {
                _root = CreateNode(new Box3d(new Vector3d(-100, 0, -100f), new Vector3d(100f, 0, 100f)), 6, entityManager);
                _first = false;
            }

            var k = _camera.Width/(Math.Tan(_camera.HorizontalFieldOfView/2));
            Draw(_root, k, entityManager, 0);
        }

        private void Draw(Node root, double k, EntityManager entityManager, int i)
        {
            var error = root.GeometricError;//Math.Pow(2, 6 - i);
            
            var distance = (root.Bounds.Center - _camera.Position).Length;
            var rho = (error/distance)*k;

            var threshhold = 100;
            if (rho <= threshhold || root.Leafs.Length == 0)
            {
                var mesh = entityManager.GetComponent<StaticMesh>(root.Entity);
                mesh.IsVisible = true;
                return;
            }

            for (int j = 0; j < root.Leafs.Length; j++)
            {
                Draw(root.Leafs[j], k, entityManager, i + 1);    
            }
        }
    }

    public class Node
    {
        public Node(Box3d bounds, Node[] leafNodes, Entity entity, double geometricError)
        {
            Entity = entity;
            GeometricError = geometricError;
            Bounds = bounds;
            Leafs = leafNodes;
        }

        public Entity Entity { get; private set; }
        public double GeometricError { get; private set; }
        public Box3d Bounds { get; private set; }
        public Node[] Leafs { get; private set; }
    }

    public struct Box3d
    {
        public Box3d(Vector3d min, Vector3d max)
            : this()
        {
            Min = min;
            Max = max;
        }

        public Vector3d Min { get; private set; }
        public Vector3d Max { get; private set; }

        public Vector3d Center
        {
            get { return (Max + Min) / 2; }
        }
    }
}