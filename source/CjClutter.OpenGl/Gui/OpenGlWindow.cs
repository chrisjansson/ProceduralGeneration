using System;
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
        private RenderSystem _renderSystem;
        private InputSystem _inputSystem;
        private Texture _texture;
        private readonly AwesomiumGui _awesomiumGui;
        private TerrainSystem _terrainSystem;
        private LightMoverSystem _lightMoverSystem;
        private WaterSystem _waterSystem;

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

            _entityManager = new EntityManager();
            _terrainSystem = new TerrainSystem(FractalBrownianMotionSettings.Default);
            _renderSystem = new RenderSystem(_camera);
            _inputSystem = new InputSystem(_keyboardInputProcessor, _camera);
            _lightMoverSystem = new LightMoverSystem();
            _waterSystem = new WaterSystem();

            const int numberOfChunksX = 10;
            const int numberOfChunksY = 10;
            for (var i = 0; i < numberOfChunksX; i++)
            {
                for (var j = 0; j < numberOfChunksY; j++)
                {
                    var entity = new Entity(Guid.NewGuid().ToString());
                    _entityManager.Add(entity);
                    _entityManager.AddComponentToEntity(entity, new ChunkComponent(i, j));
                    _entityManager.AddComponentToEntity(entity, new StaticMesh());
                    //_entityManager.AddComponentToEntity(entity, new NormalComponent());
                }
            }

            var water = new Entity(Guid.NewGuid().ToString());
            _entityManager.Add(water);
            _entityManager.AddComponentToEntity(water, new WaterComponent(10, 10));

            var light = new Entity(Guid.NewGuid().ToString());
            _entityManager.Add(light);
            _entityManager.AddComponentToEntity(light, new PositionalLightComponent { Position = new Vector3d(0, 1, 0) });

            _awesomiumGui.Start();
            _awesomiumGui.SettingsChanged += s => _terrainSystem.SetTerrainSettings(s);
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
            if (_texture == null)
            {
                _texture = new Texture();
                _texture.Create();
            }

            _frameTimeCounter.UpdateFrameTime(e.Time);

            GL.Clear(ClearBufferMask.DepthBufferBit);

            _inputSystem.Update(ElapsedTime.TotalSeconds, _entityManager);
            _terrainSystem.Update(ElapsedTime.TotalSeconds, _entityManager);
            _waterSystem.Update(ElapsedTime.TotalSeconds, _entityManager);
            _renderSystem.Update(ElapsedTime.TotalSeconds, _entityManager);
            _lightMoverSystem.Update(ElapsedTime.TotalSeconds, _entityManager);

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
