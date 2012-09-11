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
    }
}
