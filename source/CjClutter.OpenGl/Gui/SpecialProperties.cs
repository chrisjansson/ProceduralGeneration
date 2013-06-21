using System.Drawing;
using Gwen.Control;

namespace CjClutter.OpenGl.Gui
{
    public class SpecialProperties : Properties
    {
        public SpecialProperties(Base parent) : base(parent) {}

        protected override void Render(Gwen.Skin.Base skin)
        {
            base.Render(skin);

            var renderBounds = RenderBounds;

            skin.Renderer.DrawColor = Color.Brown;
            skin.Renderer.DrawFilledRect(renderBounds);
        }
    }
}