module CDLodOpenGl

open Rendering
open OpenTK.Graphics.OpenGL4
open CjClutter.OpenGl.EntityComponent
open CjClutter.OpenGl.OpenGl.VertexTypes

let makeSquareXZMesh dimension =
    let mesh = MeshCreator.CreateXZGrid(dimension, dimension)
    
    let vertexBuffer = GL.GenBuffer()
    let elementBuffer = GL.GenBuffer()

    GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer)
    let vertexDataSize = nativeint ((new Vertex3V3N()).SizeInBytes * mesh.Vertices.Length)
    GL.BufferData(BufferTarget.ArrayBuffer, vertexDataSize, mesh.Vertices, BufferUsageHint.StaticDraw)
    GL.BindBuffer(BufferTarget.ArrayBuffer, 0)

    GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBuffer)
    let elementDataSize = nativeint (sizeof<uint16> * mesh.Faces.Length * 3)

    let elements = mesh.Faces |> Array.collect (fun f -> [| f.V0; f.V1; f.V2 |] ) |> Array.map (fun e -> uint16 e)
    GL.BufferData(BufferTarget.ElementArrayBuffer, elementDataSize, elements, BufferUsageHint.StaticDraw)
    GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0)
    { ElementCount = elements.Length; VertexBuffer = vertexBuffer; ElementBuffer = elementBuffer }
