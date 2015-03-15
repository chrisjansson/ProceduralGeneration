module primitives
open OpenTK

type mesh = { 
        vertices : Vector3 []
        indices : uint16 []
    }
    with
    member this.verticesSize = this.vertices.Length * sizeof<Vector3>
    member this.elementSize = this.indices.Length * sizeof<uint16>

let unitCube = { 
        vertices = 
            [| 
                new Vector3(-0.5f, -0.5f, -0.5f) //back
                new Vector3(0.5f, -0.5f, -0.5f)
                new Vector3(0.5f, 0.5f, -0.5f)
                new Vector3(-0.5f, 0.5f, -0.5f)
                new Vector3(-0.5f, -0.5f, 0.5f) //front
                new Vector3(0.5f, -0.5f, 0.5f)
                new Vector3(0.5f, 0.5f, 0.5f)
                new Vector3(-0.5f, 0.5f, 0.5f)
            |]
        indices = 
            [| 
                4; 6; 7; //front
                4; 5; 6;
                6; 5; 1; //right
                6; 1; 2;
                0; 4; 7; //left
                0; 7; 3;
                0; 2; 1; //back
                0; 3; 2;
                7; 6; 2; //top
                7; 2; 3;
                4; 1; 5; //bottom
                4; 0; 1;
            |] |> Array.map (fun i -> uint16 i)
    }

type V3N3 = 
    struct
        val Vertex : Vector3
        val Normal : Vector3
        new(vertex : Vector3, normal : Vector3) = { Vertex = vertex; Normal = normal }
    end

type meshWithNormals = {
        vertices : V3N3 []
    }

let unitPlane =
    let vertices = [ 
        new Vector3(-0.5f, 0.0f, 0.5f)
        new Vector3(0.5f, 0.0f, 0.5f)
        new Vector3(0.5f, 0.0f, -0.5f)
        new Vector3(-0.5f, 0.0f, 0.5f)
        new Vector3(0.5f, 0.0f, -0.5f)
        new Vector3(-0.5f, 0.0f, -0.5f)]

    {
        meshWithNormals.vertices = vertices |> List.map (fun v -> new V3N3(v, Vector3.UnitY)) |> List.toArray
    }

let transformV3N3 (vn:V3N3) (transform:Matrix4) =
    let transformVector v =
        Vector3.Transform(v, transform)
    let transformNormal n =
        Vector3.TransformNormal(n, transform)
    new V3N3(transformVector vn.Vertex, transformNormal vn.Normal)

let transformMesh m (transform:Matrix4) = 
    { vertices = m.vertices |> Array.map (fun vn -> transformV3N3 vn transform) }

let unitCubeWithNormals =
    let pi = float32 System.Math.PI
    let transforms = [| 
        Matrix4.CreateTranslation(0.0f, 0.5f, 0.0f) 
        Matrix4.CreateRotationX(pi) * Matrix4.CreateTranslation(0.0f, -0.5f, 0.0f)
        Matrix4.CreateRotationX(pi / 2.0f) * Matrix4.CreateTranslation(0.0f, 0.0f, 0.5f)
        Matrix4.CreateRotationX(-pi / 2.0f) * Matrix4.CreateTranslation(0.0f, 0.0f, -0.5f)
        Matrix4.CreateRotationZ(pi / 2.0f) * Matrix4.CreateTranslation(-0.5f, 0.0f, 0.0f)
        Matrix4.CreateRotationZ(-pi / 2.0f) * Matrix4.CreateTranslation(0.5f, 0.0f, 0.0f) |]
    { vertices = transforms |> Array.collect (fun t -> (transformMesh unitPlane t).vertices) }