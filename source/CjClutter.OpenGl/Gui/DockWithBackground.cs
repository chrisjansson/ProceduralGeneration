using System.Drawing;
using Gwen.Control;

namespace CjClutter.OpenGl.Gui
{
    public class DockWithBackground : DockBase
    {
        public DockWithBackground(Base parent) : base(parent)
        {
            BackgroundColor = Color.White;
        }

        public Color BackgroundColor { get; set; }

        protected override void Render(Gwen.Skin.Base skin)
        {
            bool wasResized = SizeToChildren(false, true);
            while (wasResized)
            {
                wasResized = SizeToChildren(false, true);
            }

            base.Render(skin);

            if (ShouldDrawBackground)
            {
                var renderer = skin.Renderer;


                var oldColor = renderer.DrawColor;
                {
                    renderer.DrawColor = BackgroundColor;
                    renderer.DrawFilledRect(RenderBounds);
                }
                renderer.DrawColor = oldColor;
            }
        }
    }
}