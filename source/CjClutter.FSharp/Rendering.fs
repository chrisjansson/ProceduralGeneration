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

type IndividualRenderJob = {
        IndividualContext : IndividualRenderContext
        Mesh : meshWithNormals
    }

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


