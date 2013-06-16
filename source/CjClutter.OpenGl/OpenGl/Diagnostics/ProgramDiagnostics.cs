using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;

namespace CjClutter.OpenGl.OpenGl.Diagnostics
{
    public class ProgramDiagnostics
    {
        public Boolean GetLinkStatus(Program program)
        {
            int linkStatus;
            GL.GetProgram(program.ProgramId, ProgramParameter.LinkStatus, out linkStatus);

            return (Boolean)linkStatus;
        }

        public IEnumerable<UniformInfo> GetActiveUniforms(IProgram program)
        {
            var uniforms = new List<UniformInfo>();

            var numberOfActiveUniforms = program.GetNumberOfActiveUniforms();
            for (var uniformIndex = 0; uniformIndex < numberOfActiveUniforms; uniformIndex++)
            {
                var uniform = GetActiveUniform(program.ProgramId, uniformIndex);
                uniforms.Add(uniform);
            }

            return uniforms;
        } 

        private UniformInfo GetActiveUniform(int program, int uniformIndex)
        {
            int uniformSize;
            ActiveUniformType activeUniformType;
            var name = GL.GetActiveUniform(program, uniformIndex, out uniformSize, out activeUniformType);

            return new UniformInfo(name, uniformSize, activeUniformType);
        }
    }

    public class UniformInfo
    {
        private readonly string _name;
        private readonly int _size;
        private readonly ActiveUniformType _type;

        public UniformInfo(string name, int size, ActiveUniformType type)
        {
            _type = type;
            _size = size;
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }

        public int Size
        {
            get { return _size; }
        }

        public ActiveUniformType Type
        {
            get { return _type; }
        }
    }
}
