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

    GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBuffer)
    let elements = mesh.Faces |> Array.collect (fun f -> [| f.V0; f.V1; f.V2 |] ) |> Array.map (fun e -> uint16 e)
    let elementDataSize = nativeint (sizeof<uint16> * elements.Length)
    GL.BufferData(BufferTarget.ElementArrayBuffer, elementDataSize, elements, BufferUsageHint.StaticDraw)

    { ElementCount = elements.Length; VertexBuffer = vertexBuffer; ElementBuffer = elementBuffer }

let createVertexArrayObjectForV3N3 (vertexVertexAttribute:int) (normalVertexAttribute:int) =
    let vertexArray = GL.GenVertexArray()
    let v = new Vertex3V3N()

    GL.BindVertexArray(vertexArray)
    GL.EnableVertexAttribArray(vertexVertexAttribute)
    GL.EnableVertexAttribArray(normalVertexAttribute)

    GL.VertexAttribPointer(vertexVertexAttribute, 3, VertexAttribPointerType.Float, false, v.SizeInBytes, v.PositionOffset)
    GL.VertexAttribPointer(normalVertexAttribute, 3, VertexAttribPointerType.Float, false, v.SizeInBytes, v.NormalOffset)
    GL.BindVertexArray(0)
    vertexArray

type RenderableCDLodMesh = {
        Bind : unit -> unit
        ElementCount : int
    }

let makeRenderableSquareXZMesh dimension vertexVertexAttribute normalVertexAttribute =
    let allocatedMesh = makeSquareXZMesh dimension
    let vertexArrayObject = createVertexArrayObjectForV3N3 vertexVertexAttribute normalVertexAttribute
    GL.BindBuffer(BufferTarget.ArrayBuffer, 0)
    GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0)
    
    {
        Bind = fun _ -> 
            GL.BindVertexArray(vertexArrayObject)
            GL.BindBuffer(BufferTarget.ArrayBuffer, allocatedMesh.VertexBuffer)
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, allocatedMesh.ElementBuffer)
        ElementCount = allocatedMesh.ElementCount
    }


    
