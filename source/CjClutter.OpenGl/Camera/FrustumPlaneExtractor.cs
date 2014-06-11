using OpenTK;

namespace CjClutter.OpenGl.Camera
{
    public class FrustumPlaneExtractor
    {
        public static Vector4d[] ExtractRowMajor(ICamera camera)
        {
            var matrix = camera.ComputeCameraMatrix() * camera.ComputeProjectionMatrix();

            var left = matrix.Column3 + matrix.Column0;
            var right = matrix.Column3 - matrix.Column0;
            var bottom = matrix.Column3 + matrix.Column1;
            var top = matrix.Column3 - matrix.Column1;
            var near = matrix.Column3 + matrix.Column2;
            var far = matrix.Column3 - matrix.Column2;

            return new[]
            {
                NormalizePlane(left),
                NormalizePlane(right),
                NormalizePlane(bottom),
                NormalizePlane(top),
                NormalizePlane(near),
                NormalizePlane(far),
            };
        }

        private static Vector4d NormalizePlane(Vector4d left)
        {
            var magnitude = left.Xyz.Normalized().Length;
            return left / magnitude;
        }
    }
}