using Gwen.Control;
using Gwen.Skin;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace CjClutter.OpenGl.Gui
{
    public class GwenGuiBase
    {
        private readonly Gwen.Renderer.OpenTK _renderer;
        private Matrix4 _projectionMatrix;
        private readonly TexturedBase _skin;
        protected Canvas Root;
        private bool _isEnabled;

        public GwenGuiBase()
        {
            _renderer = new Gwen.Renderer.OpenTK();
            _skin = new TexturedBase(_renderer, "DefaultSkin.png");

            Root = new Canvas(_skin);
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                var changed = _isEnabled != value;
                _isEnabled = value;

                if(changed)
                    OnEnabledChanged();
            }
        }

        public void Resize(int width, int height)
        {
            _projectionMatrix = Matrix4.CreateOrthographicOffCenter(0, width, height, 0, -1, 1);
            Root.SetBounds(0, 0, width, height);
        }

        public void Draw()
        {
            if(!IsEnabled) return;

            MaintainTextCache();

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref _projectionMatrix);

            Root.RenderCanvas();
        }

        private void MaintainTextCache()
        {
            if (_renderer.TextCacheSize > 1000)
            {
                _renderer.FlushTextCache();
            }
        }

        public void Close()
        {
            _renderer.Dispose();
            _skin.Dispose();
            Root.Dispose();
        }

        protected virtual void OnEnabledChanged() {}
    }
}