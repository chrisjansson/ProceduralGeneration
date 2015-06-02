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

uniform vec4 pParam = vec4(289.0, 34.0, 1.0, 7.0);

vec3 permute(vec3 x0,vec3 p) { 
	vec3 x1 = mod(x0 * p.y, p.x);
	return floor(  mod( (x1 + p.z) *x0, p.x ));
}

float simplex_noise2(vec2 v) {
	const vec2 C = vec2(0.211324865405187134, // (3.0-sqrt(3.0))/6.;
		                0.366025403784438597); // 0.5*(sqrt(3.0)-1.);
	const vec3 D = vec3( 0., 0.5, 2.0) * 3.14159265358979312;

	// First corner
	vec2 i  = floor(v + dot(v, C.yy) );
	vec2 x0 = v -   i + dot(i, C.xx);

	// Other corners
	vec2 i1  =  (x0.x > x0.y) ? vec2(1.,0.) : vec2(0.,1.) ;

	//  x0 = x0 - 0. + 0. * C
	vec2 x1 = x0 - i1 + 1. * C.xx ;
	vec2 x2 = x0 - 1. + 2. * C.xx ;

	// Permutations
	i = mod(i, pParam.x);
	vec3 p = permute( permute( 
		     i.y + vec3(0., i1.y, 1. ), pParam.xyz)
		   + i.x + vec3(0., i1.x, 1. ), pParam.xyz);

	// ( N points uniformly over a line, mapped onto a diamond.)
	vec3 x = fract(p / pParam.w) ;
	vec3 h = 0.5 - abs(x) ;

	vec3 sx = vec3(lessThan(x,D.xxx)) *2. -1.;
	vec3 sh = vec3(lessThan(h,D.xxx));

	vec3 a0 = x + sx*sh;
	vec2 p0 = vec2(a0.x,h.x);
	vec2 p1 = vec2(a0.y,h.y);
	vec2 p2 = vec2(a0.z,h.z);
	vec3 g = 2.0 * vec3( dot(p0, x0), dot(p1, x1), dot(p2, x2) );

	// mix
	vec3 m = max(0.5 - vec3(dot(x0,x0), dot(x1,x1), dot(x2,x2)), 0.);
	m = m*m ;
	return 1.66666* 70.*dot(m*m, g);
}

float noise3d(vec3 pos) {
    return simplex_noise2(pos.xy);
}

float offset = 1.0;
int octaves = 7;
float lacunarity = 1.0;
float gain = 1.0;
float H = 1.0;

float ridgedMultiFractal(vec3 pos) {
    float signal = noise3d(pos);
    signal = abs(signal);
    signal = offset - signal;
    signal *= signal;
    float result = signal;

    float frequency = 1.0;
    for(int i = 1; i < octaves; i++) {
        pos = lacunarity * pos;
        float weight = clamp(signal * gain, 0.0, 1.0);
        signal = noise3d(pos);
        signal = abs(signal);
        signal = offset - signal;
        signal *= signal;
        signal *= weight;

        result += signal * pow(frequency, -H);

        frequency *= lacunarity;
    }
    return result;
}

int width = 128;
int height = width;

vec2 max = vec2(0.0);
vec2 min = vec2(0.0);

vec2 getPosition(int x, int y) {
    vec2 delta = max - min;

    float columnFraction = float(x) / float(width);
    float rowFraction = float(y) / float(height);

    return vec2(
        min.x + delta.x * columnFraction,
        min.y + delta.y * rowFraction);
}

float getHeight(int x, int y) {
    vec2 position = getPosition(x, y);
    return ridgedMultiFractal(vec3(position, 0.0));
}

vec3 getNormal(int x, int y) {
    vec2 center = CalculatePosition(x, y);
}
//            public Vector3d GetNormal(int column, int row)
//            {
//                var center = CalculatePosition(column, row);
//                double d = 1;
//                var leftRight = new Vector3d(d * 2, _noiseGenerator.Noise(center.X + d, center.Y) - _noiseGenerator.Noise(center.X - d, center.Y), 0);
//                var bottomTop = new Vector3d(0, _noiseGenerator.Noise(center.X, center.Y - d) - _noiseGenerator.Noise(center.X, center.Y + d), d * 2);
//
//                var normal = -(Vector3d.Cross(leftRight.Normalized(), bottomTop.Normalized()).Normalized());
//                return normal;
//            }

void main() {
    for(int x = 0; x <= width; x++) {
        for(int y = 0; y <= height; y++) {
            vec3 pos = vec3(x, getHeight(x, y), y);
            //vec3 normal = getNormal(x, y);
            //transform pos and normal
            //save
        }
    }
    Positions[gl_GlobalInvocationID.x] = ridgedMultiFractal(vec3(gl_GlobalInvocationID.x, 0.0, 0.0));
}
"

let rawShader = [ (ShaderType.ComputeShader, computeShaderSource) ]

let makeNoiseShader =
    match makeProgram rawShader with
    | Some programId -> programId
    | _ -> failwith "Program compilation failed"



