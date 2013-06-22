using Gwen;
using Gwen.Control;
using OpenTK;

namespace CjClutter.OpenGl.Gui
{
    public class Hud : GwenGuiBase
    {
        private readonly Label _frameTimeLabel;
        private readonly Label _fpsLabel;
        
        public Hud(INativeWindow gameWindow)
        {
            Root.SetBounds(0, 0, gameWindow.Width, gameWindow.Height);

            _fpsLabel = new Label(Root)
                {
                    AutoSizeToContents = true,
                    Dock = Pos.Top,
                };

            _frameTimeLabel = new Label(Root)
                {
                    AutoSizeToContents = true,
                    Dock = Pos.Top,
                };
        }

        public void Update(double elapsed, double frameTime)
        {
            UpdateControls(elapsed, frameTime);
        }

        private double _deadLine;
        private void UpdateControls(double elapsed, double frameTime)
        {
            if (_deadLine > elapsed)
                return;

            _fpsLabel.Text = string.Format("{0:0}fps", 1 / frameTime);
            _frameTimeLabel.Text = string.Format("{0:0}ms", frameTime * 1000);
            _deadLine = elapsed + 1;
        }
    }
}