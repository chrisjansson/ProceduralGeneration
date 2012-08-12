using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using CjClutter.OpenGl.Noise;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using QuickFont;

namespace CjClutter.OpenGl
{
    public class OpenGlWindow : GameWindow
    {
        private const int TerrainWidth = 256;
        private const int TerrainHeight = 256;

        private readonly Dictionary<Key, Action<GameWindow>> _keyboardInputActions = new Dictionary<Key, Action<GameWindow>>
                                                                                         {
                                                                                             { Key.Escape, w => w.Exit() },
                                                                                         };

        private QFont _qFont;
        private readonly FrameTimeCounter _frameTimeCounter = new FrameTimeCounter();
        private INoiseGenerator _noiseGenerator;
        private Stopwatch _stopwatch;
        private double[,] _heightMap;

        public OpenGlWindow(int width, int height, string title, OpenGlVersion openGlVersion)
            : base(
            width,
            height,
            GraphicsMode.Default,
            title,
            GameWindowFlags.Default,
            DisplayDevice.Default,
            openGlVersion.Major,
            openGlVersion.Minor,
            GraphicsContextFlags.Default)
        {
            VSync = VSyncMode.Off;
        }

        protected override void OnLoad(EventArgs e)
        {
            var font = new Font(FontFamily.GenericSansSerif, 10);
            _qFont = QFontFactory.Create(font);

            _heightMap = new double[TerrainWidth, TerrainHeight];

            _noiseGenerator = new SimplexNoise();
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            ProcessKeyboardInput();

            _frameTimeCounter.UpdateFrameTime(e.Time);

            var perspectiveMatrix = Matrix4d.CreatePerspectiveFieldOfView(Math.PI/4, 1, 1, 100);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perspectiveMatrix);

            var lookAtMatrix = Matrix4d.LookAt(2, 2, 0, 0, 0, 0, 0, 1, 0);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref lookAtMatrix);

            GL.ClearColor(Color4.White);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.Color3(Color.Green);

            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            GL.Begin(BeginMode.Quads);

            var elapsedseconds = _stopwatch.Elapsed.TotalSeconds / 10;

            for (var i = 0; i < TerrainWidth; i++)
            {
                for (var j = 0; j < TerrainHeight; j++)
                {
                    var xin = i/(double) TerrainWidth*2;
                    var yin = j/(double) TerrainHeight*2;
                    _heightMap[i, j] = 0.2 * _noiseGenerator.Noise(xin, yin, elapsedseconds);
                }
            }

            for (var i = 0; i < TerrainWidth - 1; i++)
            {
                for (var j = 0; j < TerrainHeight - 1; j++)
                {
                    var x0 = -0.5 + ScaleTo(i, TerrainWidth);
                    var x1 = -0.5 + ScaleTo(i + 1, TerrainWidth);
                    var x2 = -0.5 + ScaleTo(i + 1, TerrainWidth);
                    var x3 = -0.5 + ScaleTo(i, TerrainWidth);

                    var y0 = -0.5 + ScaleTo(j, TerrainHeight);
                    var y1 = -0.5 + ScaleTo(j, TerrainHeight);
                    var y2 = -0.5 + ScaleTo(j + 1, TerrainHeight);
                    var y3 = -0.5 + ScaleTo(j + 1, TerrainHeight);

                    var z0 = _heightMap[i, j];
                    var z1 = _heightMap[i + 1, j];
                    var z2 = _heightMap[i + 1, j + 1];
                    var z3 = _heightMap[i, j + 1];

                    GL.Vertex3(x0, z0, y0);
                    GL.Vertex3(x1, z1, y1);
                    GL.Vertex3(x2, z2, y2);
                    GL.Vertex3(x3, z3, y3);
                }
            }
        
            GL.End();

            GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);

            DrawDebugText();

            SwapBuffers();
        }

        private double ScaleTo(double value, double max)
        {
            return value / max;
        }

        private void DrawDebugText()
        {
            QFontExtensions.RunInQFontScope(() =>
                {
                    _qFont.ResetVBOs();
                    _qFont.PrintToVBO(_frameTimeCounter.ToOutputString(), Vector2.Zero, Color.Black);
                    _qFont.LoadVBOs();
                    _qFont.DrawVBOs();
                });
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