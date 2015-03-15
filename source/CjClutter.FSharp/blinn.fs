#version 400 

uniform mat3 normalMatrix;
uniform mat4 viewMatrix;
uniform mat4 modelMatrix;

uniform vec3 ambientColor;
uniform vec3 diffuseColor;
uniform vec3 specColor;

out vec4 outColor;
in vec3 vNormal;
in vec3 vPosition;

const vec4 lightPosition = vec4(1.0);
const float specularExp = 150.0;

vec3 calculateDiffuse(vec3 diffuseColor, vec3 dirToLight, vec3 normal) {
    float incidence = dot(dirToLight, normal);
    return max(0.0, incidence) * diffuseColor; 
}

vec3 calculateSpecular(vec3 dirToLight, vec3 dirToEye, vec3 normal) {
    vec3 h = normalize(dirToLight + dirToEye);
    float specularAngle = max(0.0, dot(h, normal));
    return specColor * pow(specularAngle, specularExp);
}

void main()
{
    vec3 normal = normalize(normalMatrix * vNormal);
    vec4 position = viewMatrix * modelMatrix * vec4(vPosition, 1.0);

    vec3 dirToLight = normalize((viewMatrix * lightPosition - position).xyz);
    vec3 diffuse = calculateDiffuse(diffuseColor, dirToLight, normal);

    vec3 dirToEye = normalize(-(position.xyz));
    vec3 specular = calculateSpecular(dirToLight, dirToEye, normal);

    outColor = vec4(ambientColor + diffuse + specular, 1.0);
}
