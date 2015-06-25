module Terrain

open Rendering
open OpenTK
open CjClutter.OpenGl
open CjClutter.OpenGl.OpenGl
open CjClutter.OpenGl.Gui
open CjClutter.OpenGl.EntityComponent
open OpenTK.Graphics.OpenGL4

type node = ChunkedLodTreeFactory.ChunkedLodTreeNode

let resourceFactory = new ResourceAllocator(new OpenGlResourceFactory())

let allocate (node:node) =
    let factory = new TerrainChunkFactory()
    let mesh = factory.Create(node.Bounds)
    let allocatedMesh = resourceFactory.AllocateResourceFor(mesh)
    let bounds = node.Bounds
    let translation = Matrix4.CreateTranslation(float32 bounds.Center.X, 0.0f, float32 bounds.Center.Y)
    let delta = bounds.Max - bounds.Min
    let scale = Matrix4.CreateScale(float32 delta.X, 1.0f, float32 delta.Y)
    let bind () =
        allocatedMesh.CreateVAO()
        allocatedMesh.VertexArrayObject.Bind()
    { 
        bind = bind
        faces = mesh.Faces.Length
        renderContext = {
                            ModelMatrix = scale * translation
                            NormalMatrix = Matrix3.Identity
                        }
    }
   
let allocateGpu (noiseShader:NoiseShaderProgram.NoiseShader) (node:node) =
    GL.UseProgram(noiseShader.ProgramId)

    let bounds = node.Bounds

    let max = new Vector2(float32 bounds.Max.X, float32 bounds.Max.Y)
    let min = new Vector2(float32 bounds.Min.X, float32 bounds.Min.Y)
    noiseShader.Max.set max
    noiseShader.Min.set min
    let widthInPoints = 128
    let meshDimensions = float32 (widthInPoints - 1)
    let numberOfPoints = 128 * 128
    noiseShader.Transform.set (Matrix4.CreateTranslation(-meshDimensions / 2.0f, 0.0f, -meshDimensions / 2.0f) * Matrix4.CreateScale(1.0f / meshDimensions, 1.0f, 1.0f / meshDimensions))
    noiseShader.NormalTransform.set OpenTK.Matrix3.Identity
    GL.DispatchCompute(numberOfPoints / 128, 1, 1)


type NoiseShaderProgram = 
    {
        programId : int
    }

let allocateOnGpu (programId:int) (node:node) =
    let buffer = GL.GenBuffer()
    GL.BindBuffer(BufferTarget.ShaderStorageBuffer, buffer)
    GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 4, buffer)
    GL.UseProgram(programId)
    GL.DispatchCompute(1, 1, 1)
    let bounds = node.Bounds
    let translation = Matrix4.CreateTranslation(float32 bounds.Center.X, 0.0f, float32 bounds.Center.Y)
    let delta = bounds.Max - bounds.Min
    let scale = Matrix4.CreateScale(float32 delta.X, 1.0f, float32 delta.Y)
    {
        bind = fun () -> ()
        faces = 1
        renderContext = {
                            ModelMatrix = scale * translation
                            NormalMatrix = Matrix3.Identity
                        }
    }

let makeTerrainLodTree =
    CjClutter.OpenGl.Terrain.CreateTree()
