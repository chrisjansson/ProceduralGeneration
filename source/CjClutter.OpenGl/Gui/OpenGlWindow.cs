using System;
using System.Diagnostics;
using System.Drawing;
using CjClutter.OpenGl.Camera;
using CjClutter.OpenGl.CoordinateSystems;
using CjClutter.OpenGl.Input;
using CjClutter.OpenGl.Input.Keboard;
using CjClutter.OpenGl.Input.Mouse;
using CjClutter.OpenGl.SceneGraph;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using QuickFont;

namespace CjClutter.OpenGl.Gui
{
    public class OpenGlWindow : GameWindow
    {
        private QFont _qFont;
        private readonly FrameTimeCounter _frameTimeCounter = new FrameTimeCounter();
        private Stopwatch _stopwatch;
        private readonly MouseInputProcessor _mouseInputProcessor;
        private readonly MouseInputObservable _mouseInputObservable;
        private readonly KeyboardInputProcessor _keyboardInputProcessor = new KeyboardInputProcessor();
        private readonly KeyboardInputObservable _keyboardInputObservable;
        private readonly OpenTkCamera _openTkCamera;
        private readonly Scene _scene;

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

            _mouseInputProcessor = new MouseInputProcessor(this, new GuiToRelativeCoordinateTransformer());

            var buttonUpEventEvaluator = new ButtonUpActionEvaluator(_mouseInputProcessor);
            _mouseInputObservable = new MouseInputObservable(buttonUpEventEvaluator);

            _keyboardInputObservable = new KeyboardInputObservable(_keyboardInputProcessor);

            var trackballCameraRotationCalculator = new TrackballCameraRotationCalculator();
            var trackballCamera = new TrackballCamera(trackballCameraRotationCalculator);
            _openTkCamera = new OpenTkCamera(_mouseInputProcessor, trackballCamera);

            _scene = new Scene();
        }

        protected override void OnLoad(EventArgs e)
        {
            var font = new Font(FontFamily.GenericSansSerif, 10);
            _qFont = QFontFactory.Create(font);

            _stopwatch = new Stopwatch();
            _stopwatch.Start();

            var esc = new KeyArg(Key.Escape);
            var altEnter = new KeyArg(Key.AltLeft, Key.Enter);
            _keyboardInputObservable.SubscribeKey(esc, KeyArgDirection.Down, Exit);
            _keyboardInputObservable.SubscribeKey(altEnter, KeyArgDirection.Down | KeyArgDirection.Up, ToggleFullScren);

            GL.Color3(Color.Green);

            _scene.OnLoad();
        }

        private void ToggleFullScren()
        {
            if (WindowState == WindowState.Fullscreen)
            {
                WindowState = WindowState.Normal;
            }
            else if(WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Fullscreen;
            }
        }

        protected override void OnUnload(EventArgs e)
        {
            _scene.OnUnload();
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            ProcessMouseInput();
            ProcessKeyboardInput();

            _frameTimeCounter.UpdateFrameTime(e.Time);

            var perspectiveMatrix = Matrix4d.CreatePerspectiveFieldOfView(Math.PI / 4, 1, 1, 100);

            GL.ClearColor(Color4.White);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _scene.ViewMatrix = _openTkCamera.GetCameraMatrix();
            _scene.ProjectionMatrix = perspectiveMatrix;
            _scene.Update(ElapsedTime.TotalSeconds);
            _scene.Draw();

            DrawDebugText();

            SwapBuffers();
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
            if (!Focused)
            {
                return;
            }

            var keyboardState = OpenTK.Input.Keyboard.GetState();

            _keyboardInputProcessor.Update(keyboardState);
            _keyboardInputObservable.ProcessKeys();
        }

        private void ProcessMouseInput()
        {
            if (!Focused)
            {
                return;
            }

            var mouseState = OpenTK.Input.Mouse.GetState();

            _mouseInputProcessor.Update(mouseState);
            _mouseInputObservable.ProcessMouseButtons();

            _openTkCamera.Update();
        }

        public TimeSpan ElapsedTime
        {
            get
            {
                return _stopwatch.Elapsed;
            }
        }
    }
}