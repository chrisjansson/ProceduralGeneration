using Gwen;
using Gwen.Control;
using Gwen.Skin;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace CjClutter.OpenGl.SceneGraph
{
    public class Hud
    {
        private readonly Label _frameTimeLabel;
        private readonly Label _fpsLabel;
        private readonly Canvas _canvas;
        private readonly Gwen.Renderer.OpenTK _renderer;
        private readonly TexturedBase _texturedBase;
        private Matrix4 _projectionMatrix;

        public Hud(GameWindow gameWindow)
        {
            _renderer = new Gwen.Renderer.OpenTK();
            _texturedBase = new TexturedBase(_renderer, "DefaultSkin.png");
            _canvas = new Canvas(_texturedBase);

            //var input = new Gwen.Input.OpenTK(gameWindow);
            //input.Initialize(_canvas);

            //_canvas.ShouldDrawBackground = true;
            //_canvas.BackgroundColor = Color.FromArgb(255, 150, 170, 170);

            _fpsLabel = new Label(_canvas)
                {
                    AutoSizeToContents = true, 
                    Dock = Pos.Top
                };

            _frameTimeLabel = new Label(_canvas)
                {
                    AutoSizeToContents = true, 
                    Dock = Pos.Top
                };
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

        private double _deadLine = 0;
        private void UpdateControls(double elapsed, double frameTime)
        {
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