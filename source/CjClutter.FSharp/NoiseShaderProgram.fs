module NoiseShaderProgram

open shader
open OpenTK
open OpenTK.Graphics
open OpenTK.Graphics.OpenGL
    
let computeShaderSource = 
    "#version 430
uniform image2D destTex;

void main() {
    
}
"

let rawShader = [ (ShaderType.ComputeShader, computeShaderSource) ]

let makeNoiseShader =
    match makeProgram rawShader with
    | Some programId -> programId
    | _ -> failwith "Program compilation failed"



