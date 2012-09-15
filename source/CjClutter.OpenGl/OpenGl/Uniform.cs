using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace CjClutter.OpenGl.OpenGl
{
    public class Uniform
    {
        private readonly int _location;

        public Uniform(int location)
        {
            _location = location;
        }

        public void Set(ref Matrix4 matrix)
        {
            GL.UniformMatrix4(_location, false, ref matrix);
        }

        public void Set(ref Vector3 vector)
        {
            GL.Uniform3(_location, ref vector);
        }

        public void Set(ref Vector4 vector)
        {
            GL.Uniform4(_location, ref vector);
        }
    }
}
