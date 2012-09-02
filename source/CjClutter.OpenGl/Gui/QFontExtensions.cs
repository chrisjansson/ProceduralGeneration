using System;
using QuickFont;

namespace CjClutter.OpenGl.Gui
{
    public static class QFontExtensions
    {
        public static void RunInQFontScope(Action qFontAction)
        {
            QFont.Begin();
            qFontAction();
            QFont.End();
        }
    }
}