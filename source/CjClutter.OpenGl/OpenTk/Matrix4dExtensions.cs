using OpenTK;

namespace CjClutter.OpenGl.OpenTk
{
    public static class Matrix4dExtensions
    {
        public static Matrix4d Multiply(this Matrix4d left, Matrix4d right)
        {
            Matrix4d result;
            Matrix4d.Mult(ref left, ref right, out result);

            return result;
        }

        public static Matrix4d GetRotationMatrix(this Quaterniond quaternion)
        {
            return Matrix4d.Rotate(quaternion);
        }

        public static Matrix4 ToMatrix4(this Matrix4d matrix)
        {
            var floatMatrix = new Matrix4(
                (float) matrix.M11,
                (float) matrix.M12,
                (float) matrix.M13,
                (float) matrix.M14,
                (float) matrix.M21,
                (float) matrix.M22,
                (float) matrix.M23,
                (float) matrix.M24,
                (float) matrix.M31,
                (float) matrix.M32,
                (float) matrix.M33,
                (float) matrix.M34,
                (float) matrix.M41,
                (float) matrix.M42,
                (float) matrix.M43,
                (float) matrix.M44);

            return floatMatrix;
        }
    }
}
