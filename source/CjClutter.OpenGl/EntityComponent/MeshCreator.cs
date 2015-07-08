using System.Collections.Generic;
using CjClutter.OpenGl.OpenGl.VertexTypes;
using CjClutter.OpenGl.SceneGraph;
using OpenTK;

namespace CjClutter.OpenGl.EntityComponent
{
    public class MeshCreator
    {
        public static Mesh3V3N CreateXZGrid(int columns, int rows)
        {
            return CreateFromHeightMap(columns, rows, new FlatHeightMap());
        }

        public static Mesh3V3N CreateFromHeightMap(int columns, int rows, IHeightMap heightMap)
        {
            var vertices = new List<Vertex3V3N>();
            for (var x = 0; x <= columns; x++)
            {
                for (var y = 0; y <= rows; y++)
                {
                    var height = heightMap.GetHeight(x, y);
                    vertices.Add(new Vertex3V3N
                    {
                        Position = new Vector3(x, (float)height, y),
                        Normal = (Vector3)heightMap.GetNormal(x, y)
                    });
                }
            }

            var faces = CreateFaces(columns, rows);

            return new Mesh3V3N(vertices, faces).Transformed(Matrix4.CreateTranslation(-columns / 2f, 0, -rows / 2f) * Matrix4.CreateScale((float)(1.0 / columns), 1, (float)(1.0 / rows)));
        }

        public static List<Face3> CreateFaces(int columns, int rows)
        {
            var faces = new List<Face3>();
            for (var x = 0; x < columns; x++)
            {
                for (var y = 0; y < rows; y++)
                {
                    var verticesInColumn = rows + 1;
                    var v0 = x * verticesInColumn + y;
                    var v1 = (x + 1) * verticesInColumn + y;
                    var v2 = (x + 1) * verticesInColumn + y + 1;
                    var v3 = x * verticesInColumn + y + 1;

                    Face3 f0;
                    Face3 f1;
                    if (y % 2 == 0)
                    {
                        if (x % 2 == 0)
                        {
                            f0 = new Face3 { V0 = v0, V1 = v1, V2 = v2 };
                            f1 = new Face3 { V0 = v0, V1 = v2, V2 = v3 };
                        }
                        else
                        {
                            f0 = new Face3 { V0 = v0, V1 = v1, V2 = v3 };
                            f1 = new Face3 { V0 = v1, V1 = v2, V2 = v3 };
                        }
                    }
                    else
                    {
                        if (x % 2 == 0)
                        {
                            f0 = new Face3 { V0 = v0, V1 = v1, V2 = v3 };
                            f1 = new Face3 { V0 = v1, V1 = v2, V2 = v3 };
                        }
                        else
                        {
                            f0 = new Face3 { V0 = v0, V1 = v1, V2 = v2 };
                            f1 = new Face3 { V0 = v0, V1 = v2, V2 = v3 };
                        }
                    }

                    faces.Add(f0);
                    faces.Add(f1);
                }
            }

            return faces;
        }

        private class FlatHeightMap : IHeightMap
        {
            public double GetHeight(int column, int row)
            {
                return 0;
            }

            public Vector3d GetNormal(int column, int row)
            {
                return new Vector3d(0, 1, 0);
            }
        }
    }

    public interface IHeightMap
    {
        double GetHeight(int column, int row);
        Vector3d GetNormal(int column, int row);
    }
}

