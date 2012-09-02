using System.Drawing;
using QuickFont;

namespace CjClutter.OpenGl.Gui
{
    public class QFontFactory
    {
        public static QFont Create(Font font)
        {
            var config = new QFontBuilderConfiguration
                             {
                UseVertexBuffer = true,
            };

            return new QFont(font, config);
        } 
    }
}