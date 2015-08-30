#version 400 

uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;
uniform mat4 modelMatrix;
uniform float morphStart;
uniform float morphEnd;
uniform vec3 cameraPosition;

layout(location = 0) in vec3 position;
layout(location = 1) in vec3 normal;

out vec3 vNormal;
out vec3 vPosition;

vec2 g_gridDim = vec2(64.0, 64.0);
vec2 g_quadScale = vec2(1.0, 1.0);

vec2 morphVertex(vec2 gridPos, vec2 vertex, float morphK)
{
	vec2 fracPart = fract(gridPos.xy * g_gridDim.xy * 0.5) * 2.0 / g_gridDim.xy;
	return vertex.xy - fracPart * g_quadScale.xy * morphK;
} 

void main()
{
	vec4 worldPosition = modelMatrix * vec4(position, 1.0);
	float l = length(worldPosition.xyz - cameraPosition);
	float clamped = clamp(l - morphStart, morphStart, morphEnd);
	float a = (clamped - morphStart) /  (morphEnd - morphStart);
 	vec2 morphedPos = morphVertex(position.xz + vec2(0.5),  worldPosition.xz, 1.0 - a);
	gl_Position = projectionMatrix * viewMatrix * vec4(morphedPos.x, 0.0, morphedPos.y, 1.0);
    vPosition = position;
    vNormal = normal;
}

