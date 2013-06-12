using System;
using CjClutter.OpenGl.Camera;
using CjClutter.OpenGl.SceneGraph;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace CjClutter.OpenGl.Gui
{
    public class Renderer
    {
        private ProjectionMode _projectionMode = ProjectionMode.Perspective;
        private Matrix4d _projectionMatrix;

        public void Render(Scene scene, ICamera camera)
        {
            var cameraMatrix = camera.GetCameraMatrix();
            scene.ViewMatrix = cameraMatrix;
            scene.ProjectionMatrix = _projectionMatrix;

            GL.ClearColor(Color4.White);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            scene.Draw();
        }

        public void Resize(int width, int height)
        {
            _projectionMatrix = CreateProjectionMatrix(width, height);
        }

        private Matrix4d CreateProjectionMatrix(int width, int height)
        {
            if (_projectionMode == ProjectionMode.Perspective)
            {
                return Matrix4d.CreatePerspectiveFieldOfView(Math.PI / 4, (double)width / height, 1, 100);
            }

            return Matrix4d.CreateOrthographic(2, 2, 1, 100);
        }

        public void SetProjectionMode(ProjectionMode projectionMode)
        {
            _projectionMode = projectionMode;
        }
    }
}