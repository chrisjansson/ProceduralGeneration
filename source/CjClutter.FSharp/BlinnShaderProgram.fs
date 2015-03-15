module BlinnShaderProgram
open OpenTK.Graphics.OpenGL
open shader

let vertexShaderSource = System.IO.File.ReadAllText("Blinn.vs")
let fragmentShaderSource = System.IO.File.ReadAllText("Blinn.fs")

let rawShaders = [ 
    (ShaderType.VertexShader, vertexShaderSource); 
    (ShaderType.FragmentShader, fragmentShaderSource) ]

type BlinnPhongProgram = {
        ProgramId : int
        ProjectionMatrixUniform : MatrixUniform
        ViewMatrix : MatrixUniform
        ModelMatrix : MatrixUniform
        NormalMatrix : Matrix3Uniform
        AmbientColor : Vector3Uniform
        DiffuseColor : Vector3Uniform
        SpecularColor : Vector3Uniform
    }

let makeBlinnShaderProgram =
    match makeProgram rawShaders with
    | Some programId -> 
        { 
            ProgramId = programId; 
            ProjectionMatrixUniform = makeMatrixUniform programId "projectionMatrix"
            ViewMatrix = makeMatrixUniform programId "viewMatrix"
            ModelMatrix = makeMatrixUniform programId "modelMatrix"
            NormalMatrix = makeMatrix3Uniform programId "normalMatrix"
            AmbientColor = makeVector3Uniform programId "ambientColor"
            DiffuseColor = makeVector3Uniform programId "diffuseColor"
            SpecularColor = makeVector3Uniform programId "specColor"
        }
    | _ -> failwith "Program compilation failed"

