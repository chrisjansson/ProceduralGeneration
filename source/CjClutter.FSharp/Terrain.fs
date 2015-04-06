module Terrain

open Rendering
open CjClutter.OpenGl
open CjClutter.OpenGl.OpenGl
open CjClutter.OpenGl.Gui
open CjClutter.OpenGl.EntityComponent

type node = ChunkedLodTreeFactory.ChunkedLodTreeNode

let resourceFactory = new ResourceAllocator(new OpenGlResourceFactory())

let allocate (node:node) =
    let factory = new TerrainChunkFactory()
    let mesh = factory.Create(node.Bounds)
    let allocatedMesh = resourceFactory.AllocateResourceFor(mesh)
    let bind () =
        allocatedMesh.CreateVAO()
        allocatedMesh.VertexArrayObject.Bind()
    { bind = bind }

let makeTerrainLodTree =
    CjClutter.OpenGl.Terrain.CreateTree()
