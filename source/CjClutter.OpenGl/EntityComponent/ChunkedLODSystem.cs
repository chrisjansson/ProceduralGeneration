using System;
using CjClutter.OpenGl.SceneGraph;
using OpenTK;

namespace CjClutter.OpenGl.EntityComponent
{
    public class ChunkedLODSystem : IEntitySystem
    {
        public ChunkedLODSystem()
        {
            _root = GetNode(new Box2(new Vector2(-0.5f, -0.5f), new Vector2(0.5f, 0.5f)), 5);
        }

        private Node GetNode(Box2 bounds, int level)
        {
            if (level == 0)
            {
                return new Node(bounds, new Node[] { });
            }

            var centerHeight = bounds.Top + bounds.Height / 2;
            var centerWidth = bounds.Left + bounds.Width / 2;
            var center = new Vector2(centerWidth, centerHeight);

            return new Node(bounds,
                new[]
                {
                    GetNode(new Box2(new Vector2(bounds.Left, bounds.Top), center), level -1),
                    GetNode(new Box2(new Vector2(centerWidth, bounds.Top), new Vector2(bounds.Right, centerHeight)), level -1),
                    GetNode(new Box2(new Vector2(bounds.Left, centerHeight), new Vector2(centerWidth, bounds.Bottom)), level -1),
                    GetNode(new Box2(center, new Vector2(bounds.Right, bounds.Bottom)), level -1)
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

            var centerHeight = root.Bounds.Top + root.Bounds.Height / 2;
            var centerWidth = root.Bounds.Left + root.Bounds.Width / 2;
            var center = new Vector2(centerWidth, centerHeight);

            var mesh = GridCreator.CreateXZ(10, 10);
            var staticMesh = new StaticMesh()
            {
                Color = new Vector4(0f, 0f, 1f, 1f),
                ModelMatrix = Matrix4.Identity,

            };

            //staticMesh.Update(mesh.Transformed(Matrix4.Mult(Matrix4.CreateTranslation(new Vector3(center.X, 0, center.Y)), Matrix4.CreateScale(0.5f, 0.5f, 0.5f))));
            staticMesh.Update(mesh.Transformed(Matrix4.CreateScale(root.Bounds.Width, root.Bounds.Height, root.Bounds.Width) * Matrix4.CreateTranslation(new Vector3(center.X, 0, center.Y))));

            var entity = new Entity(Guid.NewGuid().ToString());
            entityManager.Add(entity);
            entityManager.AddComponentToEntity(entity, staticMesh);
        }
    }

    public class Node
    {
        public Node(Box2 bounds, Node[] leafNodes)
        {
            Bounds = bounds;
            Leafs = leafNodes;
        }

        public Box2 Bounds { get; private set; }
        public Node[] Leafs { get; private set; }
    }

    public struct Box3d
    {
        public Box3d(Vector3d min, Vector3d max) : this()
        {
            Min = min;
            Max = max;
        }

        public Vector3d Min { get; private set; }
        public Vector3d Max { get; private set; }

        public Vector3d Center
        {
            get { return (Max + Min)/2; }
        }
    }
}