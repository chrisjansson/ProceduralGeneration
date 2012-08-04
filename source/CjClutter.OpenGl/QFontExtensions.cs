using System;
using QuickFont;

namespace CjClutter.OpenGl
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