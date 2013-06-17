using System.Drawing;
using OpenTK;

namespace CjClutter.OpenGl.SceneGraph
{
    public class ColorCycle
    {
        private int _next;
        private readonly Color[] _colors;

        public ColorCycle()
        {
            _colors = new[]
                {
                    Color.Aqua,
                    Color.Blue,
                    Color.CadetBlue,
                    Color.Chartreuse,
                    Color.CornflowerBlue,
                    Color.Crimson,
                    Color.Cyan,
                    Color.DarkCyan,
                    Color.DarkOliveGreen,
                    Color.Goldenrod,
                    Color.GreenYellow,
                    Color.Lime,
                    Color.OrangeRed,
                    Color.PaleTurquoise,
                    Color.SeaGreen,
                    Color.Yellow,
                };
        }

        public Vector4 GetNext()
        {
            var color = _colors[_next];
            _next = (_next + 1)%_colors.Length;


            return new Vector4(Scale(color.R), Scale(color.G), Scale(color.B), 1.0f);
        }

        private float Scale(byte component)
        {
            return component/255.0f;
        }
    }
}