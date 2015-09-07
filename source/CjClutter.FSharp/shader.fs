module shader

open Result
open OpenTK
open OpenTK.Graphics.OpenGL

type shaderCompilationStatus = 
    | Success of shaderId : int
    | Error of message : string

let getShaderCompilationError (shaderId:int) =
    GL.GetShaderInfoLog(shaderId)

let getShaderCompilationStatus (shaderId:int) =
    let mutable status = -1
    GL.GetShader(shaderId, ShaderParameter.CompileStatus, &status)
    let convertedStatus = enum status
    match convertedStatus with
        | Boolean.True -> Success shaderId
        | _ -> Error (getShaderCompilationError shaderId)

let compileShader shaderType source =
    let shaderId = GL.CreateShader shaderType
    GL.ShaderSource(shaderId, source)
    GL.CompileShader(shaderId)
    getShaderCompilationStatus shaderId

type shadersCompilationResult =
    | Success of shaderIds : list<int>
    | Error of errors : list<string>

let compileShaders shaders =
    let seed = Success(List.empty)
    shaders 
        |> List.map (fun (t, s) -> compileShader t s)
        |> List.fold (fun state compiledShader ->  
            match (state, compiledShader) with
            | (Success shaderIds, shaderCompilationStatus.Success shaderId) -> Success(shaderId::shaderIds)
            | (Success _, shaderCompilationStatus.Error message) -> Error([message])
            | (Error messages, shaderCompilationStatus.Success _) -> Error(messages)
            | (Error messages, shaderCompilationStatus.Error message) -> Error(message::messages))
            seed

let linkProgram shaders =
    let programId = GL.CreateProgram()
    for shaderId in shaders do
        GL.AttachShader(programId, shaderId)
    GL.LinkProgram programId
    programId

let makeProgram shaders =
    match compileShaders shaders with
    | Success shaderIds -> Result.Success (linkProgram shaderIds)
    | Error messages -> Failure (messages |> List.reduce (fun l r -> l + " " + r))

let glSetUniform uniformId (matrix:Matrix4d byref) =
    GL.UniformMatrix4(uniformId, false, &matrix)

let uniformSetterForMatrix4d uniformId =
    fun (matrix:Matrix4) -> 
        let mutable m = matrix
        GL.UniformMatrix4(uniformId, false, &m)

type MatrixUniform  = { set : OpenTK.Matrix4 -> unit } 

let uniformSetterForMatrix3 uniformId =
    fun (matrix:Matrix3) ->
        let mutable m = matrix
        GL.UniformMatrix3(uniformId, false, &m)

type Matrix3Uniform = { set : OpenTK.Matrix3 -> unit }

let makeMatrixUniform (programId:int) uniformName =
    let uniformLocation = GL.GetUniformLocation(programId, uniformName)
    { MatrixUniform.set = uniformSetterForMatrix4d uniformLocation }

let makeMatrix3Uniform (programId:int) uniformName =
    let uniformLocation = GL.GetUniformLocation(programId, uniformName)
    { Matrix3Uniform.set = uniformSetterForMatrix3 uniformLocation }

type Vector3Uniform = { set : OpenTK.Vector3 -> unit }

let makeVector3Uniform (programId:int) uniformName =
    let uniformLocation = GL.GetUniformLocation(programId, uniformName)
    { Vector3Uniform.set = fun v -> GL.Uniform3(uniformLocation, v) }

type Vector2Uniform = { set : OpenTK.Vector2 -> unit }

let makeVector2Uniform (programId:int) uniformName =
    let uniformLocation = GL.GetUniformLocation(programId, uniformName)
    { Vector2Uniform.set = fun v -> GL.Uniform2(uniformLocation, v) }

type FloatUniform = { set : float32 -> unit }

let makeFloatUniform (programId:int) uniformName =
    let uniformLocation = GL.GetUniformLocation(programId, uniformName)
    { FloatUniform.set = fun f -> GL.Uniform1(uniformLocation, f) }

type IntUniform = { set : int -> unit }

let makeIntUniform (programId:int) uniformName =
    let uniformLocation = GL.GetUniformLocation(programId, uniformName)
    { IntUniform.set = fun f -> GL.Uniform1(uniformLocation, f) }

let loadTexture fileName =
    let textureId = GL.GenTexture()
    GL.BindTexture(TextureTarget.Texture2D, textureId)
    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
    let texture = System.IO.File.ReadAllBytes(fileName)
    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, 4096, 4096, 0, PixelFormat.Red, PixelType.UnsignedShort, texture)
    textureId