module NormalDebugShaderProgram
open OpenTK.Graphics.OpenGL
open shader

let vertexShaderSource =
    "#version 400 

layout(location = 0) in vec3 position;
layout(location = 1) in vec3 normal;

out vec3 vPosition;
out vec3 vNormal;

void main()
{
    vPosition = position;
    vNormal = normal;
}"

let geometryShaderSource =
    "#version 400 

layout(points) in;
layout(line_strip, max_vertices = 2) out;

in vec3 vPosition[];
in vec3 vNormal[];

uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;
uniform mat4 modelMatrix;
uniform mat3 normalMatrix;

vec4 transformedPosition;
vec4 transformedNormal;

void main() {
    transformedPosition = projectionMatrix * viewMatrix * modelMatrix * vec4(vPosition[0], 1.0);
    transformedNormal = vec4(normalize(normalMatrix * vNormal[0]), 0.0);

    gl_Position = transformedPosition;
    EmitVertex();

    gl_Position = transformedPosition + projectionMatrix * (transformedNormal * 0.5);
    EmitVertex();

    EndPrimitive();
}"

let fragmentShaderSource = 
    "#version 400 

out vec4 outColor;

void main()
{
    outColor = vec4(0.2, 0.2, 1.0, 1.0);
}"

let rawShaders = [ 
    (ShaderType.VertexShader, vertexShaderSource)
    (ShaderType.GeometryShader, geometryShaderSource)
    (ShaderType.FragmentShader, fragmentShaderSource) ]

type SimpleProgram = {
        ProgramId : int
        ProjectionMatrixUniform : MatrixUniform
        ViewMatrix : MatrixUniform
        ModelMatrix : MatrixUniform
        NormalMatrix : Matrix3Uniform
    }

let makeSimpleShaderProgram =
    match makeProgram rawShaders with
    | Some programId -> 
        { 
            ProgramId = programId; 
            ProjectionMatrixUniform = makeMatrixUniform programId "projectionMatrix"
            ViewMatrix = makeMatrixUniform programId "viewMatrix"
            ModelMatrix = makeMatrixUniform programId "modelMatrix"
            NormalMatrix = makeMatrix3Uniform programId "normalMatrix"
        }
    | _ -> failwith "Program compilation failed"



