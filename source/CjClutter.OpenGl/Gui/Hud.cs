using Gwen;
using Gwen.Control;
using Gwen.Skin;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace CjClutter.OpenGl.Gui
{
    public class Hud
    {
        private readonly Label _frameTimeLabel;
        private readonly Label _fpsLabel;
        private readonly Canvas _canvas;
        private readonly Gwen.Renderer.OpenTK _renderer;
        private readonly TexturedBase _texturedBase;
        private readonly GeneratoionSettingsControl _generatoionSettingsControl;
        private Matrix4 _projectionMatrix;

        public Hud(GameWindow gameWindow)
        {
            _renderer = new Gwen.Renderer.OpenTK();
            _texturedBase = new TexturedBase(_renderer, "DefaultSkin.png");

            _canvas = new Canvas(_texturedBase);
            _canvas.SetBounds(0, 0, gameWindow.Width, gameWindow.Height);

            var input = new Gwen.Input.OpenTK(gameWindow);
            input.Initialize(_canvas);

            _fpsLabel = new Label(_canvas)
                {
                    AutoSizeToContents = true,
                    Dock = Pos.Top,
                    IsHidden = true,
                };

            _frameTimeLabel = new Label(_canvas)
                {
                    AutoSizeToContents = true,
                    Dock = Pos.Top,
                    IsHidden = true
                };

            _generatoionSettingsControl = new GeneratoionSettingsControl(_canvas);

            gameWindow.Mouse.Move += (sender, args) => input.ProcessMouseMessage(args);
            gameWindow.Mouse.ButtonDown += (sender, args) => input.ProcessMouseMessage(args);
            gameWindow.Mouse.ButtonUp += (sender, args) => input.ProcessMouseMessage(args);
            gameWindow.Mouse.WheelChanged += (sender, args) => input.ProcessMouseMessage(args);

            gameWindow.Keyboard.KeyDown += (sender, args) => input.ProcessKeyDown(args);
            gameWindow.Keyboard.KeyUp += (sender, args) => input.ProcessKeyUp(args);
        }

        public void Resize(int width, int height)
        {
            _projectionMatrix = Matrix4.CreateOrthographicOffCenter(0, width, height, 0, -1, 1);
        }

        public void Update(double elapsed, double frameTime)
        {
            MaintainTextCache();

            UpdateControls(elapsed, frameTime);
        }

        private void MaintainTextCache()
        {
            if (_renderer.TextCacheSize > 1000)
            {
                _renderer.FlushTextCache();
            }
        }

        private double _deadLine;
        private void UpdateControls(double elapsed, double frameTime)
        {
            _generatoionSettingsControl.Update();

            if (_deadLine > elapsed)
                return;

            _fpsLabel.Text = string.Format("{0:0}fps", 1 / frameTime);
            _frameTimeLabel.Text = string.Format("{0:0}ms", frameTime * 1000);
            _deadLine = elapsed + 1;
        }

        public void Draw()
        {
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref _projectionMatrix);

            _canvas.RenderCanvas();
        }

        public void Close()
        {
            _renderer.Dispose();
            _texturedBase.Dispose();
            _canvas.Dispose();
        }
    }
}