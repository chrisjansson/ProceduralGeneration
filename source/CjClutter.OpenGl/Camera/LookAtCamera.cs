using System;
using OpenTK;

namespace CjClutter.OpenGl.Camera
{
    public class LookAtCamera : ICamera
    {
        private ProjectionMode _projection = ProjectionMode.Perspective;

        public LookAtCamera()
        {
            Position = new Vector3d(0, 0, 2);
            Target = new Vector3d(0, 0, 0);
            Up = new Vector3d(0, 1, 0);
        }

        public Vector3d Position { get; set; }
        public Vector3d Target { get; set; }
        public Vector3d Up { get; set; }

        public ProjectionMode Projection
        {
            get { return _projection; }
            set { _projection = value; }
        }

        public Matrix4d ComputeCameraMatrix()
        {
            return Matrix4d.LookAt(Position, Target, Up);
        }

        public Matrix4d ComputeProjectionMatrix(double width, double height)
        {
            return Projection.ComputeProjectionMatrix(width, height);
        }
    }

    //The coordinate system is right handed by default following old OpenGL conventions
    //The projection matrix produced by opentk flips z values which aso inverts the handedness of the coordinate system 
    //the projection matrix transforms to clip coordinates and later transformed to NDC which are left handed
    public abstract class ProjectionMode
    {
        public static readonly ProjectionMode Orthographic = new OrthographicProjection();
        public static readonly ProjectionMode Perspective = new PerspectiveProjection();

        public const int NearPlane = 1;
        public const int FarPlane = 100;

        public abstract Matrix4d ComputeProjectionMatrix(double width, double height);

        private class PerspectiveProjection : ProjectionMode
        {
            private const double FieldOfView = Math.PI / 4;

            public override Matrix4d ComputeProjectionMatrix(double width, double height)
            {
                var aspectRatio = width / height;

                return Matrix4d.CreatePerspectiveFieldOfView(FieldOfView, aspectRatio, NearPlane, FarPlane);
            }
        }

        private class OrthographicProjection : ProjectionMode
        {
            private const double CameraWidth = 2;
            private const double CameraHeight = 2;

            public override Matrix4d ComputeProjectionMatrix(double width, double height)
            {
                return Matrix4d.CreateOrthographic(CameraWidth, CameraHeight, NearPlane, FarPlane);
            }
        }
    }
}