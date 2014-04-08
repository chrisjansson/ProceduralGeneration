using System;
using System.Diagnostics;
using CjClutter.OpenGl.Camera;
using CjClutter.OpenGl.CoordinateSystems;
using CjClutter.OpenGl.EntityComponent;
using CjClutter.OpenGl.Input;
using CjClutter.OpenGl.Input.Keboard;
using CjClutter.OpenGl.Input.Mouse;
using CjClutter.OpenGl.Noise;
using CjClutter.OpenGl.SceneGraph;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using FrameEventArgs = OpenTK.FrameEventArgs;

namespace CjClutter.OpenGl.Gui
{
    public class OpenGlWindow : GameWindow
    {
        private readonly FrameTimeCounter _frameTimeCounter = new FrameTimeCounter();
        private Stopwatch _stopwatch;
        private readonly MouseInputProcessor _mouseInputProcessor;
        private readonly MouseInputObservable _mouseInputObservable;
        private readonly KeyboardInputProcessor _keyboardInputProcessor = new KeyboardInputProcessor();
        private readonly KeyboardInputObservable _keyboardInputObservable;
        private readonly OpentkTrackballCameraControls _opentkTrackballCameraControls;
        private readonly Hud _hud;
        private readonly Renderer _renderer;
        private readonly Menu _menu;
        private readonly ICamera _camera;
        private EntityManager _entityManager;
        private RenderSystem _renderSystem;
        private InputSystem _inputSystem;

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
            //VSync = VSyncMode.Off;

            _mouseInputProcessor = new MouseInputProcessor(this, new GuiToRelativeCoordinateTransformer());

            var buttonUpEventEvaluator = new ButtonUpActionEvaluator(_mouseInputProcessor);
            _mouseInputObservable = new MouseInputObservable(buttonUpEventEvaluator);

            _keyboardInputObservable = new KeyboardInputObservable(_keyboardInputProcessor);

            var trackballCameraRotationCalculator = new TrackballCameraRotationCalculator();
            _camera = new LookAtCamera();
            var trackballCamera = new TrackballCamera(_camera, trackballCameraRotationCalculator);
            _opentkTrackballCameraControls = new OpentkTrackballCameraControls(_mouseInputProcessor, trackballCamera);

            _renderer = new Renderer();
            _hud = new Hud(this);
            //_menu = new Menu(this, _scene);
            //_menu.GenerationSettingsControl.SetSettings(new FractalBrownianMotionSettings(6, 0.5, 0.6));
        }

        protected override void OnLoad(EventArgs e)
        {
            _stopwatch = new Stopwatch();
            _stopwatch.Start();

            //update mouse and keyboard processors always, then run update depending on wether the menu or renderer is active

            //Input for both
            _keyboardInputObservable.SubscribeKey(KeyCombination.LeftAlt && KeyCombination.Enter, CombinationDirection.Down, ToggleFullScren);

            //Inputs for "game"
            _keyboardInputObservable.SubscribeKey(KeyCombination.Esc, CombinationDirection.Down, Exit);
            _keyboardInputObservable.SubscribeKey(KeyCombination.P, CombinationDirection.Down, () => _camera.Projection = ProjectionMode.Perspective);
            _keyboardInputObservable.SubscribeKey(KeyCombination.O, CombinationDirection.Down, () => _camera.Projection = ProjectionMode.Orthographic);
            _keyboardInputObservable.SubscribeKey(KeyCombination.Tilde, CombinationDirection.Down, () => _menu.IsEnabled = !_menu.IsEnabled);

            //Inputs for "menu"
            _entityManager = new EntityManager();
            _renderSystem = new RenderSystem(_camera);
            _inputSystem = new InputSystem(_keyboardInputProcessor, _camera);

            var terrainGenerator = new TerrainGenerator(FractalBrownianMotionSettings.Default);
            var staticMeshes = terrainGenerator.Generate();
            foreach (var staticMesh in staticMeshes)
            {
                var name = Guid.NewGuid().ToString();
                var entity = new Entity(name);
                _entityManager.Add(entity);
                _entityManager.AddComponentToEntity(entity, staticMesh);
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            _hud.Close();
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
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            _camera.Width = Width;
            _camera.Height = Height;
            _renderer.Resize(Width, Height);
            _hud.Resize(Width, Height);
            //_menu.Resize(Width, Height);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            ProcessMouseInput();
            ProcessKeyboardInput();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            _frameTimeCounter.UpdateFrameTime(e.Time);

            _inputSystem.Update(ElapsedTime.TotalSeconds, _entityManager);
            _renderSystem.Update(ElapsedTime.TotalSeconds, _entityManager);

            GL.Clear(ClearBufferMask.DepthBufferBit);
            //_hud.Update(ElapsedTime.TotalSeconds, _frameTimeCounter.FrameTime);
            //_hud.Draw();
            //_menu.Update();
            //_menu.Draw();

            Console.WriteLine(_frameTimeCounter.Fps);

            SwapBuffers();
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

            _opentkTrackballCameraControls.Update();
        }

        public TimeSpan ElapsedTime
        {
            get { return _stopwatch.Elapsed; }
        }
    }
}
