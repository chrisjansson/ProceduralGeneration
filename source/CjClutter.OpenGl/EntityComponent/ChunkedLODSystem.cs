using System;
using CjClutter.OpenGl.Camera;
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
            staticMesh.Update(mesh.Transformed(Matrix4.CreateScale((float)size.X, 1, (float)size.Z) * Matrix4.CreateTranslation((Vector3)bounds.Center)));

            var entity = new Entity(Guid.NewGuid().ToString());
            entityManager.Add(entity);
            entityManager.AddComponentToEntity(entity, staticMesh);

            if (level == 0)
            {
                return new Node(bounds, new Node[] { }, entity);
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
                }, entity);
        }

        private bool _first = true;
        private Node _root;
        private ICamera _camera;

        public void Update(double elapsedTime, EntityManager entityManager)
        {
            if (_first)
            {
                _root = CreateNode(new Box3d(new Vector3d(-10f, 0, -10f), new Vector3d(10f, 0, 10f)), 6, entityManager);
                _first = false;
            }

            Clear(_root, entityManager);

            var k = _camera.Width/(2*Math.Tan((Math.PI/4)/2));
            Draw(_root, k, entityManager, 0);
        }

        private void Draw(Node root, double d, EntityManager entityManager, int i)
        {
            var error = Math.Pow(2, 6 - i);
            
            var matrix = _camera.ComputeCameraMatrix()*_camera.ComputeProjectionMatrix();
            var distance  = Vector3d.Transform(root.Bounds.Center, matrix).Length;
            var rho = error/distance*d;

            var threshhold = 1000;
            if (rho <= threshhold || root.Leafs.Length == 0)
            {
                var mesh = entityManager.GetComponent<StaticMesh>(root.Entity);
                mesh.IsVisible = true;
                return;
            }

            for (int j = 0; j < root.Leafs.Length; j++)
            {
                Draw(root.Leafs[j], d, entityManager, i + 1);    
            }
        }

        private void Clear(Node node, EntityManager entityManager)
        {
            var mesh = entityManager.GetComponent<StaticMesh>(node.Entity);
            mesh.IsVisible = false;

            for (int i = 0; i < node.Leafs.Length; i++)
            {
                Clear(node.Leafs[i], entityManager);
            }
        }
    }

    public class Node
    {
        public Node(Box3d bounds, Node[] leafNodes, Entity entity)
        {
            Entity = entity;
            Bounds = bounds;
            Leafs = leafNodes;
        }

        public Entity Entity { get; private set; }
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