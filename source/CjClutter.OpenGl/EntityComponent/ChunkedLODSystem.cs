using System;
using OpenTK;

namespace CjClutter.OpenGl.EntityComponent
{
    public class ChunkedLODSystem : IEntitySystem
    {
        public ChunkedLODSystem()
        {
            _root = GetNode(new Box3d(new Vector3d(-0.5f, 0, -0.5f), new Vector3d(0.5f, 0, 0.5f)), 5);
        }

        private Node GetNode(Box3d bounds, int level)
        {
            if (level == 0)
            {
                return new Node(bounds, new Node[] { });
            }

            var min = bounds.Min;
            var max = bounds.Max;
            var center = bounds.Center;

            return new Node(bounds,
                new[]
                {
                    GetNode(new Box3d(bounds.Min, center), level -1),
                    GetNode(new Box3d(new Vector3d(center.X, 0, min.Z), new Vector3d(max.X, 0, center.Z)), level -1),
                    GetNode(new Box3d(new Vector3d(min.X, 0, center.Z), new Vector3d(center.X, 0, max.Z)), level - 1),
                    GetNode(new Box3d(center, max), level - 1)
                });
        }

        private bool _first = true;
        private Node _root;

        public void Update(double elapsedTime, EntityManager entityManager)
        {
            if (_first)
            {
                Add(_root, entityManager, 5);

                _first = false;
            }
        }

        private void Add(Node root, EntityManager entityManager, int i)
        {
            if (i > 0)
            {
                //for (int j = 0; j < root.Leafs.Length - 1; j++)
                //{
                //    var node = root.Leafs[j];
                //    Add(node, entityManager, i - 1);
                //}

                Add(root.Leafs[0], entityManager, i - 1);
                Add(root.Leafs[1], entityManager, 0);
                Add(root.Leafs[2], entityManager, 0);
                Add(root.Leafs[3], entityManager, 0);
                return;
            }

            var mesh = GridCreator.CreateXZ(10, 10);
            var staticMesh = new StaticMesh
            {
                Color = new Vector4(0f, 0f, 1f, 1f),
                ModelMatrix = Matrix4.Identity,

            };

            var size = root.Bounds.Max - root.Bounds.Min;  
            staticMesh.Update(mesh.Transformed(Matrix4.CreateScale((float) size.X, 1, (float) size.Z) * Matrix4.CreateTranslation((Vector3) root.Bounds.Center)));

            var entity = new Entity(Guid.NewGuid().ToString());
            entityManager.Add(entity);
            entityManager.AddComponentToEntity(entity, staticMesh);
        }
    }

    public class Node
    {
        public Node(Box3d bounds, Node[] leafNodes)
        {
            Bounds = bounds;
            Leafs = leafNodes;
        }

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