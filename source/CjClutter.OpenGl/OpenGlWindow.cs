using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using QuickFont;

namespace CjClutter.OpenGl
{
    public class OpenGlWindow : GameWindow
    {
        private readonly Dictionary<Key, Action<GameWindow>> _keyboardInputActions = new Dictionary<Key, Action<GameWindow>>
                                                                                         {
                                                                                             { Key.Escape, w => w.Exit() },
                                                                                         };

        private QFont _qFont;

        public OpenGlWindow()
            : base(800, 600, GraphicsMode.Default, "OpenGL Test Program", GameWindowFlags.Default, DisplayDevice.Default, 3, 1, GraphicsContextFlags.Default)
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            var font = new Font(FontFamily.GenericSansSerif, 10);
            var config = new QFontBuilderConfiguration()
            {
                UseVertexBuffer = true,
                TextGenerationRenderHint = TextGenerationRenderHint.SystemDefault
            };

            _qFont = new QFont(font, config);
            _qFont.PrintToVBO("Hello World!", new Vector3(), Color.Crimson);
            _qFont.LoadVBOs();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            ProcessKeyboardInput();

            GL.ClearColor(Color4.White);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //GL.Color3(Color.Green);

            //GL.Rotate(e.Time * 10, 0, 0, 1);

            //GL.Begin(BeginMode.Triangles);

            //GL.Vertex3(-0.5, -0.5, 0);
            //GL.Vertex3(0.5, -0.5, 0);
            //GL.Vertex3(0.5, 0.5, 0);

            //GL.End();

            //QFont.Begin();
            var frequency = 1 / e.Time;
            //_qFont.PrintToVBO(frequency.ToString("#"), new Vector3(0, 1, 0), Color.White);
            //_qFont.LoadVBOs();
            //_qFont.DrawVBOs();
            //_qFont.ResetVBOs();
            _qFont.DrawVBOs();
            //QFont.End();

            SwapBuffers();
        }

        private void ProcessKeyboardInput()
        {
            var keyboardState = OpenTK.Input.Keyboard.GetState();
            foreach (var keyboardInputActionPair in _keyboardInputActions)
            {
                if (keyboardState[keyboardInputActionPair.Key])
                {
                    keyboardInputActionPair.Value(this);
                }
            }
        }
    }
}