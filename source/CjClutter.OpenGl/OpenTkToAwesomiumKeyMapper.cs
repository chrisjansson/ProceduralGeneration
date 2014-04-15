using System.Collections.Generic;
using Awesomium.Core;
using OpenTK.Input;

namespace CjClutter.OpenGl
{
    public class OpenTkToAwesomiumKeyMapper
    {
        private readonly Dictionary<Key, VirtualKey> _virtualKeys;

        public OpenTkToAwesomiumKeyMapper()
        {
            _virtualKeys = new Dictionary<Key, VirtualKey>();
            _virtualKeys[Key.A] = VirtualKey.A;
            _virtualKeys[Key.B] = VirtualKey.B;
            _virtualKeys[Key.C] = VirtualKey.C;
            _virtualKeys[Key.D] = VirtualKey.D;
            _virtualKeys[Key.E] = VirtualKey.E;
            _virtualKeys[Key.F] = VirtualKey.F;
            _virtualKeys[Key.G] = VirtualKey.G;
            _virtualKeys[Key.H] = VirtualKey.H;
            _virtualKeys[Key.I] = VirtualKey.I;
            _virtualKeys[Key.J] = VirtualKey.J;
            _virtualKeys[Key.K] = VirtualKey.K;
            _virtualKeys[Key.L] = VirtualKey.L;
            _virtualKeys[Key.M] = VirtualKey.M;
            _virtualKeys[Key.N] = VirtualKey.N;
            _virtualKeys[Key.O] = VirtualKey.O;
            _virtualKeys[Key.P] = VirtualKey.P;
            _virtualKeys[Key.Q] = VirtualKey.Q;
            _virtualKeys[Key.R] = VirtualKey.R;
            _virtualKeys[Key.S] = VirtualKey.S;
            _virtualKeys[Key.T] = VirtualKey.T;
            _virtualKeys[Key.U] = VirtualKey.U;
            _virtualKeys[Key.V] = VirtualKey.V;
            _virtualKeys[Key.W] = VirtualKey.W;
            _virtualKeys[Key.X] = VirtualKey.X;
            _virtualKeys[Key.Y] = VirtualKey.Y;
            _virtualKeys[Key.Z] = VirtualKey.Z;
            _virtualKeys[Key.F1] = VirtualKey.F1;
            _virtualKeys[Key.F2] = VirtualKey.F2;
            _virtualKeys[Key.F3] = VirtualKey.F3;
            _virtualKeys[Key.F4] = VirtualKey.F4;
            _virtualKeys[Key.F5] = VirtualKey.F5;
            _virtualKeys[Key.F6] = VirtualKey.F6;
            _virtualKeys[Key.F7] = VirtualKey.F7;
            _virtualKeys[Key.F8] = VirtualKey.F8;
            _virtualKeys[Key.F9] = VirtualKey.F9;
            _virtualKeys[Key.F10] = VirtualKey.F10;
            _virtualKeys[Key.F11] = VirtualKey.F11;
            _virtualKeys[Key.F12] = VirtualKey.F12;
            _virtualKeys[Key.F13] = VirtualKey.F13;
            _virtualKeys[Key.F14] = VirtualKey.F14;
            _virtualKeys[Key.F15] = VirtualKey.F15;
            _virtualKeys[Key.F16] = VirtualKey.F16;
            _virtualKeys[Key.F17] = VirtualKey.F17;
            _virtualKeys[Key.F18] = VirtualKey.F18;
            _virtualKeys[Key.F19] = VirtualKey.F19;
            _virtualKeys[Key.F20] = VirtualKey.F20;
            _virtualKeys[Key.F21] = VirtualKey.F21;
            _virtualKeys[Key.F22] = VirtualKey.F22;
            _virtualKeys[Key.F23] = VirtualKey.F23;
            _virtualKeys[Key.F24] = VirtualKey.F24;
            _virtualKeys[Key.Enter] = VirtualKey.RETURN;
            _virtualKeys[Key.Escape] = VirtualKey.ESCAPE;
            _virtualKeys[Key.Keypad0] = VirtualKey.NUMPAD0;
            _virtualKeys[Key.Keypad1] = VirtualKey.NUMPAD1;
            _virtualKeys[Key.Keypad2] = VirtualKey.NUMPAD2;
            _virtualKeys[Key.Keypad3] = VirtualKey.NUMPAD3;
            _virtualKeys[Key.Keypad4] = VirtualKey.NUMPAD4;
            _virtualKeys[Key.Keypad5] = VirtualKey.NUMPAD5;
            _virtualKeys[Key.Keypad6] = VirtualKey.NUMPAD6;
            _virtualKeys[Key.Keypad7] = VirtualKey.NUMPAD7;
            _virtualKeys[Key.Keypad8] = VirtualKey.NUMPAD8;
            _virtualKeys[Key.Keypad9] = VirtualKey.NUMPAD9;
            _virtualKeys[Key.KeypadDivide] = VirtualKey.DIVIDE;
            _virtualKeys[Key.KeypadEnter] = VirtualKey.SEPARATOR;
            _virtualKeys[Key.KeypadMultiply] = VirtualKey.MULTIPLY;
            _virtualKeys[Key.KeypadPeriod] = VirtualKey.DECIMAL;
            _virtualKeys[Key.KeypadPlus] = VirtualKey.ADD;
            _virtualKeys[Key.KeypadMinus] = VirtualKey.SUBTRACT;
            _virtualKeys[Key.Number0] = VirtualKey.NUM_0;
            _virtualKeys[Key.Number1] = VirtualKey.NUM_1;
            _virtualKeys[Key.Number2] = VirtualKey.NUM_2;
            _virtualKeys[Key.Number3] = VirtualKey.NUM_3;
            _virtualKeys[Key.Number4] = VirtualKey.NUM_4;
            _virtualKeys[Key.Number5] = VirtualKey.NUM_5;
            _virtualKeys[Key.Number6] = VirtualKey.NUM_6;
            _virtualKeys[Key.Number7] = VirtualKey.NUM_7;
            _virtualKeys[Key.Number8] = VirtualKey.NUM_8;
            _virtualKeys[Key.Number9] = VirtualKey.NUM_9;
            _virtualKeys[Key.LAlt] = VirtualKey.LMENU;
            _virtualKeys[Key.RAlt] = VirtualKey.RMENU;
            _virtualKeys[Key.LWin] = VirtualKey.LWIN;
            _virtualKeys[Key.RWin] = VirtualKey.RWIN;
            _virtualKeys[Key.Left] = VirtualKey.LEFT;
            _virtualKeys[Key.Minus] = VirtualKey.OEM_MINUS;
            _virtualKeys[Key.Pause] = VirtualKey.PAUSE;
            _virtualKeys[Key.Period] = VirtualKey.OEM_PERIOD;
            _virtualKeys[Key.Plus] = VirtualKey.OEM_PLUS;
            _virtualKeys[Key.Quote] = VirtualKey.OEM_7;
            _virtualKeys[Key.ShiftLeft] = VirtualKey.LSHIFT;
            _virtualKeys[Key.ShiftRight] = VirtualKey.RSHIFT;
            _virtualKeys[Key.Sleep] = VirtualKey.SLEEP;
            _virtualKeys[Key.Space] = VirtualKey.SPACE;
            _virtualKeys[Key.Unknown] = VirtualKey.UNKNOWN;
            _virtualKeys[Key.Up] = VirtualKey.UP;
      
            _virtualKeys[Key.Home] = VirtualKey.HOME;
            _virtualKeys[Key.End] = VirtualKey.END;
            _virtualKeys[Key.Insert] = VirtualKey.INSERT;
            _virtualKeys[Key.Tab] = VirtualKey.TAB;
            _virtualKeys[Key.ControlLeft] = VirtualKey.LCONTROL;
            _virtualKeys[Key.ControlRight] = VirtualKey.RCONTROL;
            _virtualKeys[Key.Right] = VirtualKey.RIGHT;
            _virtualKeys[Key.Delete] = VirtualKey.DELETE;
            _virtualKeys[Key.BackSpace] = VirtualKey.BACK;
            _virtualKeys[Key.Clear] = VirtualKey.CLEAR;
            _virtualKeys[Key.Comma] = VirtualKey.OEM_COMMA;
            _virtualKeys[Key.Down] = VirtualKey.DOWN;
            _virtualKeys[Key.Menu] = VirtualKey.MENU;
            _virtualKeys[Key.NumLock] = VirtualKey.NUMLOCK;
            _virtualKeys[Key.PageDown] = VirtualKey.PRIOR;
            _virtualKeys[Key.PageUp] = VirtualKey.NEXT;
            _virtualKeys[Key.PrintScreen] = VirtualKey.SNAPSHOT;
            _virtualKeys[Key.ScrollLock] = VirtualKey.SCROLL;
            _virtualKeys[Key.Semicolon] = VirtualKey.OEM_1;
            _virtualKeys[Key.Slash] = VirtualKey.OEM_2;
            _virtualKeys[Key.BackSlash] = VirtualKey.OEM_5;
            _virtualKeys[Key.NonUSBackSlash] = VirtualKey.OEM_5;
            _virtualKeys[Key.BracketLeft] = VirtualKey.OEM_4;
            _virtualKeys[Key.BracketRight] = VirtualKey.OEM_6;

            //_virtualKeys[Key.Grave] = grave
            //_virtualKeys[Key.CapsLock] = VirtualKey. //modifier
        }

        public VirtualKey Map(Key key)
        {
            if (_virtualKeys.ContainsKey(key))
            {
                return _virtualKeys[key];
            }

            return VirtualKey.UNKNOWN;
        }
    }
}