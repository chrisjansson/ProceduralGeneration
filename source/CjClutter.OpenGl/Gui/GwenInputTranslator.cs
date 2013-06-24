using System;
using Gwen.Control;
using Gwen.Input;
using OpenTK;
using OpenTK.Input;
using Key = Gwen.Key;

namespace CjClutter.OpenGl.Gui
{
    public class GwenInputTranslator
    {
        private bool _altGr;
        private Canvas _canvas;
        private int _mouseX;
        private int _mouseY;

        public GwenInputTranslator(GameWindow window)
        {
            window.KeyPress += KeyPress;
        }

        public void Initialize(Canvas c)
        {
            _canvas = c;
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
                    _altGr = true;
                    return Key.Control;
                case OpenTK.Input.Key.LAlt:
                    return Key.Alt;
                case OpenTK.Input.Key.LShift:
                    return Key.Shift;
                case OpenTK.Input.Key.RControl:
                    return Key.Control;
                case OpenTK.Input.Key.RAlt:
                    if (_altGr)
                    {
                        _canvas.Input_Key(Key.Control, false);
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
            if (null == _canvas) return false;

            if (args is MouseMoveEventArgs)
            {
                var ev = args as MouseMoveEventArgs;
                int dx = ev.X - _mouseX;
                int dy = ev.Y - _mouseY;

                _mouseX = ev.X;
                _mouseY = ev.Y;

                return _canvas.Input_MouseMoved(_mouseX, _mouseY, dx, dy);
            }

            if (args is MouseButtonEventArgs)
            {
                var ev = args as MouseButtonEventArgs;
                return _canvas.Input_MouseButton((int) ev.Button, ev.IsPressed);
            }

            if (args is MouseWheelEventArgs)
            {
                var ev = args as MouseWheelEventArgs;
                return _canvas.Input_MouseWheel(ev.Delta*60);
            }

            return false;
        }


        public bool ProcessKeyDown(EventArgs args)
        {
            var ev = args as KeyboardKeyEventArgs;
            char ch = TranslateChar(ev.Key);

            if (InputHandler.DoSpecialKeys(_canvas, ch))
                return false;
            /*
            if (ch != ' ')
            {
                m_Canvas.Input_Character(ch);
            }
            */
            Key iKey = TranslateKeyCode(ev.Key);

            return _canvas.Input_Key(iKey, true);
        }

        public bool ProcessKeyUp(EventArgs args)
        {
            var ev = args as KeyboardKeyEventArgs;

            char ch = TranslateChar(ev.Key);

            Key iKey = TranslateKeyCode(ev.Key);

            return _canvas.Input_Key(iKey, false);
        }

        public void KeyPress(object sender, KeyPressEventArgs e)
        {
            _canvas.Input_Character(e.KeyChar);
        }
    }
}