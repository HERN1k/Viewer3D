#version 330 core

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec3 aColor;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

out vec4 Color;

void main(void)
{
    Color = vec4(aColor, 0.3f);
    gl_Position = vec4(aPosition, 1.0f) * model * view * projection;
}