using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace CjClutter.OpenGl
{
    public class OpenGlWindow : GameWindow
    {
        private readonly Dictionary<Key, Action<GameWindow>> _keyboardInputActions = new Dictionary<Key, Action<GameWindow>>
                                                                                         {
                                                                                             { Key.Escape, w => w.Exit() },
                                                                                         };

        public OpenGlWindow()
            : base(800, 600, GraphicsMode.Default, "OpenGL Test Program", GameWindowFlags.Default, DisplayDevice.Default, 3, 1, GraphicsContextFlags.Default) {}

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            ProcessKeyboardInput();

            GL.ClearColor(Color4.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.Color3(Color.White);

            GL.Rotate(e.Time * 10, 0, 0, 1);

            GL.Begin(BeginMode.Triangles);

            GL.Vertex3(-0.5, -0.5, 0);
            GL.Vertex3(0.5, -0.5, 0);
            GL.Vertex3(0.5, 0.5, 0);

            GL.End();

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