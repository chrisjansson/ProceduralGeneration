using System;
using System.Linq.Expressions;
using CjClutter.Commons.Reflection;

namespace CjClutter.OpenGl.OpenGl.Shaders
{
    public class RenderProgramBase
    {
        protected Program Program { get; set; }

        protected void RegisterUniform<T>(Expression<Func<GenericUniform<T>>> getter, Action<GenericUniform<T>> setter)
        {
            var uniformName = PropertyHelper.GetPropertyName(getter);
            var uniformLocation = Program.GetUniformLocation(uniformName);

            var uniform = new GenericUniform<T>(uniformLocation);
            setter(uniform);
        }

        public void Bind()
        {
            Program.Use();
        }

        public void Unbind()
        {
            Program.Unbind();
        }
    }
}