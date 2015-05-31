module NoiseShaderProgram

open shader
open OpenTK
open OpenTK.Graphics
open OpenTK.Graphics.OpenGL
    
let computeShaderSource = 
    "#version 430

layout(std140, binding = 4) buffer Pos
{
    float Positions[];
};

layout(local_size_x = 128, local_size_y = 1, local_size_z = 1) in;

void main() {
    Positions[gl_GlobalInvocationID.x] = gl_GlobalInvocationID.x;
}
"

let rawShader = [ (ShaderType.ComputeShader, computeShaderSource) ]

let makeNoiseShader =
    match makeProgram rawShader with
    | Some programId -> programId
    | _ -> failwith "Program compilation failed"



