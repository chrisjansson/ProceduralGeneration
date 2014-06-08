using System;
using System.Collections.Generic;
using System.Diagnostics;
using CjClutter.OpenGl.Camera;
using CjClutter.OpenGl.CoordinateSystems;
using CjClutter.OpenGl.EntityComponent;
using CjClutter.OpenGl.Input.Keboard;
using CjClutter.OpenGl.Input.Mouse;
using CjClutter.OpenGl.Noise;
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
        private readonly ICamera _camera;
        private EntityManager _entityManager;
        private Texture _texture;
        private readonly AwesomiumGui _awesomiumGui;
        private List<IEntitySystem> _systems;
        private LookAtCamera _lodCamera;
        private bool _synchronizeCameras = true;

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
            _mouseInputProcessor = new MouseInputProcessor(this, new GuiToRelativeCoordinateTransformer());

            var buttonUpEventEvaluator = new ButtonUpActionEvaluator(_mouseInputProcessor);
            _mouseInputObservable = new MouseInputObservable(buttonUpEventEvaluator);

            _keyboardInputObservable = new KeyboardInputObservable(_keyboardInputProcessor);

            _camera = new LookAtCamera();
            _lodCamera = new LookAtCamera();

            _awesomiumGui = new AwesomiumGui(this);
        }

        protected override void OnLoad(EventArgs e)
        {
            _stopwatch = new Stopwatch();
            _stopwatch.Start();

            _keyboardInputObservable.SubscribeKey(KeyCombination.LeftAlt && KeyCombination.Enter, CombinationDirection.Down, ToggleFullScren);

            _keyboardInputObservable.SubscribeKey(KeyCombination.Esc, CombinationDirection.Down, Exit);
            _keyboardInputObservable.SubscribeKey(KeyCombination.P, CombinationDirection.Down, () => _camera.Projection = ProjectionMode.Perspective);
            _keyboardInputObservable.SubscribeKey(KeyCombination.O, CombinationDirection.Down, () => _camera.Projection = ProjectionMode.Orthographic);
            _keyboardInputObservable.SubscribeKey(KeyCombination.Tilde, CombinationDirection.Down, () => _awesomiumGui.IsEnabled = !_awesomiumGui.IsEnabled);
            _keyboardInputObservable.SubscribeKey(KeyCombination.F, CombinationDirection.Down, () => _synchronizeCameras = !_synchronizeCameras);

            _entityManager = new EntityManager();

            _systems = new List<IEntitySystem>
            {
                new TerrainSystem(FractalBrownianMotionSettings.Default),
                new FreeCameraSystem(_keyboardInputProcessor, _mouseInputProcessor,_camera),
                new LightMoverSystem(),
                new OceanSystem(),
                new CubeMeshSystem(),
                new ChunkedLODSystem(_lodCamera),
                new RenderSystem(_camera),
            };

            var light = new Entity(Guid.NewGuid().ToString());
            _entityManager.Add(light);
            _entityManager.AddComponentToEntity(light, new PositionalLightComponent { Position = new Vector3d(0, 1, 0) });

            _awesomiumGui.Start();
            //_awesomiumGui.SettingsChanged += s => _terrainSystem.SetTerrainSettings(s);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            _awesomiumGui.Stop();
        }

        private void ToggleFullScren()
        {
            if (WindowState == WindowState.Fullscreen)
            {
                WindowState = WindowState.Normal;
            }
            else if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Fullscreen;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            _camera.Width = Width;
            _camera.Height = Height;
            _awesomiumGui.Resize(Width, Height);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            ProcessMouseInput();

            ProcessKeyboardInput();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            if (_synchronizeCameras)
            {
                _lodCamera.Position = _camera.Position;
                _lodCamera.Target = _camera.Target;
                _lodCamera.Up = _camera.Up;
                _lodCamera.Width = _camera.Width;
                _lodCamera.Height = _camera.Height;
            }

            if (_texture == null)
            {
                _texture = new Texture();
                _texture.Create();
            }

            _frameTimeCounter.UpdateFrameTime(e.Time);

            GL.Clear(ClearBufferMask.DepthBufferBit);

            foreach (var system in _systems)
            {
                system.Update(ElapsedTime.TotalSeconds, _entityManager);
            }

            if (_awesomiumGui.IsDirty)
            {
                _texture.Upload(_awesomiumGui.Frame);
                _awesomiumGui.IsDirty = false;
            }

            if (_awesomiumGui.IsEnabled)
                _texture.Render();

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
        }

        public TimeSpan ElapsedTime
        {
            get { return _stopwatch.Elapsed; }
        }
    }
}
