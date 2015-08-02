module BlinnShaderProgram
open OpenTK.Graphics.OpenGL
open shader

let vertexShaderSource = System.IO.File.ReadAllText("cdlod.vs")
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
        MorphK : FloatUniform
    }

let makeBlinnShaderProgram =
    match makeProgram rawShaders with
    | Result.Success programId -> 
        { 
            ProgramId = programId; 
            ProjectionMatrixUniform = makeMatrixUniform programId "projectionMatrix"
            ViewMatrix = makeMatrixUniform programId "viewMatrix"
            ModelMatrix = makeMatrixUniform programId "modelMatrix"
            NormalMatrix = makeMatrix3Uniform programId "normalMatrix"
            AmbientColor = makeVector3Uniform programId "ambientColor"
            DiffuseColor = makeVector3Uniform programId "diffuseColor"
            SpecularColor = makeVector3Uniform programId "specColor"
            MorphK = makeFloatUniform programId "morphK"
        }
    | Result.Failure m -> failwith ("Program compilation failed " + m)

