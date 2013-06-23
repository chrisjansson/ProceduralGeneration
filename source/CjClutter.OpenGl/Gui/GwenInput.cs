using System;
using Gwen.Control;
using Gwen.Input;
using OpenTK;
using OpenTK.Input;
using Key = Gwen.Key;

namespace CjClutter.OpenGl.Gui
{
    public class GwenInput
    {
        #region Properties

        private bool m_AltGr;
        private Canvas m_Canvas;

        private int m_MouseX;
        private int m_MouseY;

        #endregion

        #region Constructors

        public GwenInput(GameWindow window)
        {
            window.KeyPress += KeyPress;
        }

        #endregion

        #region Methods

        public void Initialize(Canvas c)
        {
            m_Canvas = c;
        }

        /// <summary>
        ///     Translates control key's OpenTK key code to GWEN's code.
        /// </summary>
        /// <param name="key">OpenTK key code.</param>
        /// <returns>GWEN key code.</returns>
        private Key TranslateKeyCode(OpenTK.Input.Key key)
        {
            switch (key)
            {
                case OpenTK.Input.Key.BackSpace:
                    return Key.Backspace;
                case OpenTK.Input.Key.Enter:
                    return Key.Return;
                case OpenTK.Input.Key.Escape:
                    return Key.Escape;
                case OpenTK.Input.Key.Tab:
                    return Key.Tab;
                case OpenTK.Input.Key.Space:
                    return Key.Space;
                case OpenTK.Input.Key.Up:
                    return Key.Up;
                case OpenTK.Input.Key.Down:
                    return Key.Down;
                case OpenTK.Input.Key.Left:
                    return Key.Left;
                case OpenTK.Input.Key.Right:
                    return Key.Right;
                case OpenTK.Input.Key.Home:
                    return Key.Home;
                case OpenTK.Input.Key.End:
                    return Key.End;
                case OpenTK.Input.Key.Delete:
                    return Key.Delete;
                case OpenTK.Input.Key.LControl:
                    m_AltGr = true;
                    return Key.Control;
                case OpenTK.Input.Key.LAlt:
                    return Key.Alt;
                case OpenTK.Input.Key.LShift:
                    return Key.Shift;
                case OpenTK.Input.Key.RControl:
                    return Key.Control;
                case OpenTK.Input.Key.RAlt:
                    if (m_AltGr)
                    {
                        m_Canvas.Input_Key(Key.Control, false);
                    }
                    return Key.Alt;
                case OpenTK.Input.Key.RShift:
                    return Key.Shift;
            }
            return Key.Invalid;
        }

        /// <summary>
        ///     Translates alphanumeric OpenTK key code to character value.
        /// </summary>
        /// <param name="key">OpenTK key code.</param>
        /// <returns>Translated character.</returns>
        private static char TranslateChar(OpenTK.Input.Key key)
        {
            if (key >= OpenTK.Input.Key.A && key <= OpenTK.Input.Key.Z)
                return (char) ('a' + ((int) key - (int) OpenTK.Input.Key.A));
            return ' ';
        }

        public bool ProcessMouseMessage(EventArgs args)
        {
            if (null == m_Canvas) return false;

            if (args is MouseMoveEventArgs)
            {
                var ev = args as MouseMoveEventArgs;
                int dx = ev.X - m_MouseX;
                int dy = ev.Y - m_MouseY;

                m_MouseX = ev.X;
                m_MouseY = ev.Y;

                return m_Canvas.Input_MouseMoved(m_MouseX, m_MouseY, dx, dy);
            }

            if (args is MouseButtonEventArgs)
            {
                var ev = args as MouseButtonEventArgs;
                return m_Canvas.Input_MouseButton((int) ev.Button, ev.IsPressed);
            }

            if (args is MouseWheelEventArgs)
            {
                var ev = args as MouseWheelEventArgs;
                return m_Canvas.Input_MouseWheel(ev.Delta*60);
            }

            return false;
        }


        public bool ProcessKeyDown(EventArgs args)
        {
            var ev = args as KeyboardKeyEventArgs;
            char ch = TranslateChar(ev.Key);

            if (InputHandler.DoSpecialKeys(m_Canvas, ch))
                return false;
            /*
            if (ch != ' ')
            {
                m_Canvas.Input_Character(ch);
            }
            */
            Key iKey = TranslateKeyCode(ev.Key);

            return m_Canvas.Input_Key(iKey, true);
        }

        public bool ProcessKeyUp(EventArgs args)
        {
            var ev = args as KeyboardKeyEventArgs;

            char ch = TranslateChar(ev.Key);

            Key iKey = TranslateKeyCode(ev.Key);

            return m_Canvas.Input_Key(iKey, false);
        }

        public void KeyPress(object sender, KeyPressEventArgs e)
        {
            m_Canvas.Input_Character(e.KeyChar);
        }

        #endregion
    }
}