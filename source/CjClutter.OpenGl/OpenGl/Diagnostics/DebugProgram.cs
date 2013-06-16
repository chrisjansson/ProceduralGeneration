using System.Linq;

namespace CjClutter.OpenGl.OpenGl.Diagnostics
{
    public class DebugProgram : IProgram
    {
        private readonly IProgram _program;
        private ILookup<string, UniformInfo> _activeUniforms;

        public DebugProgram(IProgram program)
        {
            _program = program;
        }

        public void AttachShader(IShader shader)
        {
            _program.AttachShader(shader);
        }

        public void DetachShader(IShader shader)
        {
            _program.DetachShader(shader);
        }

        public void Link()
        {
            _program.Link();
            LoadActiveUniforms();
        }

        private void LoadActiveUniforms()
        {
            var programDiagnostics = new ProgramDiagnostics();
            _activeUniforms = programDiagnostics.GetActiveUniforms(_program)
                                                .ToLookup(x => x.Name);
        }

        public void Unbind()
        {
            _program.Unbind();
        }

        public void Delete()
        {
            _program.Delete();
        }

        public int GetUniformLocation(string uniformName)
        {
            return _program.GetUniformLocation(uniformName);
        }

        public int GetAttributeLocation(string attributeName)
        {
            return _program.GetAttributeLocation(attributeName);
        }

        public int GetNumberOfActiveUniforms()
        {
            return _program.GetNumberOfActiveUniforms();
        }

        public int ProgramId
        {
            get { return _program.ProgramId; }
        }
    }
}