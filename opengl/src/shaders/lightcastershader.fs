#version 330 core
struct DirLight {
    vec3 direction;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};


struct PointLight {
    vec3 position;

    float constant;
    float linear;
    float quadratic;  

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
}; 

struct Material {
	sampler2D diffuse;
	sampler2D specular;
	float shininess;
};

#define POINT_LIGHT_NUM 3

in vec2 TexCoord;
in vec3 Normal;
in vec3 FragPos;

uniform Material material;
uniform DirLight dirLight;
uniform PointLight pointLight[POINT_LIGHT_NUM];
uniform vec3 viewPos;

out vec4 FragColor;

vec3 CalDirLight(DirLight dirLight, vec3 normal, vec3 viewpos);
vec3 CalPointLight(PointLight pointLight, vec3 normal, vec3 fragpos, vec3 viewpos);

void main()
{
    vec3 direct = CalDirLight(dirLight, Normal, viewPos);
    vec3 point = vec3(0, 0, 0);
    for (int i = 0; i < POINT_LIGHT_NUM; i++)
    {
        point += CalPointLight(pointLight[i], Normal, FragPos, viewPos);
    }
	FragColor = vec4(direct + point, 1.0f);
}

vec3 CalDirLight(DirLight dirLight, vec3 normal, vec3 viewpos)
{
    vec3 ambient = dirLight.ambient * vec3(texture(material.diffuse, TexCoord));

    vec3 norm = normalize(normal);
    vec3 lightDir = normalize(-dirLight.direction);
    float diff = max(dot(norm, lightDir), 0.0);
	vec3 diffuse = dirLight.diffuse * (diff * vec3(texture(material.diffuse, TexCoord)));

	vec3 viewDir = normalize(viewpos - FragPos);
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    vec3 specular = dirLight.specular * (spec * vec3(texture(material.specular, TexCoord)));
	
	return ambient + diffuse + specular;
}

vec3 CalPointLight(PointLight pointLight, vec3 normal, vec3 fragpos, vec3 viewpos)
{
    float dist = length(pointLight.position - fragpos);
    float attenuation = 1.0 / (pointLight.constant + pointLight.linear * dist + pointLight.quadratic * (dist * dist));

    vec3 ambient = pointLight.ambient * vec3(texture(material.diffuse, TexCoord));
    
    vec3 norm = normalize(normal);
    vec3 lightDir = normalize(pointLight.position - fragpos);
    float diff = max(dot(norm, lightDir), 0.0);
	vec3 diffuse = pointLight.diffuse * (diff * vec3(texture(material.diffuse, TexCoord)));

	vec3 viewDir = normalize(viewpos - FragPos);
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    vec3 specular = pointLight.specular * (spec * vec3(texture(material.specular, TexCoord)));
	
	return attenuation * (ambient + diffuse + specular);
}