module Rendering
open OpenTK
open primitives

type StaticRenderContext = {
        ViewMatrix : Matrix4
        ProjectionMatrix : Matrix4
    }

type AllocatedMesh = {
        ElementCount : int
        VertexBuffer : int
        ElementBuffer : int
    }

type RenderableMesh = {
            Bind : unit -> unit
            Faces : int
        }

type CDLodRenderJob = {
        MorphStart : float32
        MorphEnd : float32
        CameraPosition : Vector3
        ModelMatrix : Matrix4
        NormalMatrix : Matrix3
        Mesh : RenderableMesh
    }

type BasicRenderJob = {
        ModelMatrix : Matrix4
        NormalMatrix : Matrix3 //To view space
        Mesh : RenderableMesh
    }

type IndividualRenderJob =
    | CDLodRenderJob of CDLodRenderJob
    | BasicRenderJob of BasicRenderJob

type BlinnMaterial = {
        AmbientColor : Vector3
        DiffuseColor : Vector3
        SpecularColor : Vector3
    }

type RenderJob = {
        StaticContext : StaticRenderContext
        RenderJobs : IndividualRenderJob list
    }


