using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace CjClutter.OpenGl.OpenGl
{
    public class Program
    {
        public int ProgramId { get; private set; }

        public void Create()
        {
            ProgramId = GL.CreateProgram();
        }

        public void AttachShader(Shader shader)
        {
            GL.AttachShader(ProgramId, shader.ShaderId);
        }

        public void DetachShader(Shader shader)
        {
            GL.DetachShader(ProgramId, shader.ShaderId);
        }

        public void Link()
        {
            GL.LinkProgram(ProgramId);
        }

        public void Use()
        {
            GL.UseProgram(ProgramId);
        }

        public void Unbind()
        {
            GL.UseProgram(0);    
        }

        public void Delete()
        {
            GL.DeleteProgram(ProgramId);
        }

        public int GetUniformLocation(string uniformName)
        {
            return GL.GetUniformLocation(ProgramId, uniformName);
        }

        ////declarera egna delegate signaturer som klarar refar
        //private Dictionary<Type, Action<int, .object>> _uniformMapping = new Dictionary<Type, Action<int, object>>
        //    {
        //        {typeof(Matrix4), (location, o) => GL.UniformMatrix4(location, false, ref o) }
        //    };

        //public void Uniform<T>(string uniformName, ref T value)
        //{
            
        //}
    }
}
