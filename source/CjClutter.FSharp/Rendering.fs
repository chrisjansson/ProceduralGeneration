module Rendering
open OpenTK
open primitives

type StaticRenderContext = {
        ViewMatrix : Matrix4
        ProjectionMatrix : Matrix4
    }

type IndividualRenderContext = {
        ModelMatrix : Matrix4
        NormalMatrix : Matrix3 //To view space
    }

type AllocatedMesh = {
        ElementCount : int
        VertexBuffer : int
        ElementBuffer : int
    }

type RenderableMesh = {
            bind : unit -> unit
            faces : int
            renderContext : IndividualRenderContext
        }

type CDLodRenderJob = {
        MorphStart : float32
        MorphEnd : float32
        ModelMatrix : Matrix4
        NormalMatrix : Matrix3
    }

type BasicRenderJob = {
        IndividualContext : IndividualRenderContext
        Mesh : RenderableMesh
    }

type IndividualRenderJob =
    | CDLodRenderJob
    | BasicRenderJob

type BlinnMaterial = {
        AmbientColor : Vector3
        DiffuseColor : Vector3
        SpecularColor : Vector3
    }

type MaterialType =
    | Blinn of BlinnMaterial
    | NoMaterial

type RenderJob = {
        StaticContext : StaticRenderContext
        RenderJobs : list<IndividualRenderJob>
        Material : MaterialType
    }


