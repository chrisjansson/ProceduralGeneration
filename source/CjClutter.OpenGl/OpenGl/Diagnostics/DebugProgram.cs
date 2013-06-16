using System;
using System.Collections.Generic;
using System.Linq;

namespace CjClutter.OpenGl.OpenGl.Diagnostics
{
    public class DebugProgram : IProgram
    {
        private readonly IProgram _program;
        private Dictionary<string, UniformInfo> _activeUniforms;
        private ILogger _logger;

        public DebugProgram(IProgram program, ILogger logger)
        {
            _logger = logger;
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
                                                .ToDictionary(x => x.Name);
        }

        public void Unbind()
        {
            _program.Unbind();
        }

        public void Delete()
        {
            _program.Delete();
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

        public Uniform<T> GetUniform<T>(string uniformName)
        {
            var hasUniform = _activeUniforms.ContainsKey(uniformName);
            if (!hasUniform)
            {
                var message = string.Format("Uniform {0} does not exist in the linked program.", uniformName);
                _logger.Warn(message);
            }

            return _program.GetUniform<T>(uniformName);
        }

        public void Create()
        {
            _program.Create();
        }

        public void Use()
        {
            _program.Use();
        }
    }
}