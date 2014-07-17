using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Awesomium.Core;
using Awesomium.Core.Data;
using OpenTK;
using OpenTK.Input;
using FrameEventArgs = Awesomium.Core.FrameEventArgs;

namespace CjClutter.OpenGl.Gui
{
    public class Frame
    {
        public byte[] Buffer { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Frame(byte[] buffer, int width, int height)
        {
            Buffer = buffer;
            Width = width;
            Height = height;
        }
    }

    public abstract class AwesomiumGui
    {
        private readonly OpenTkToAwesomiumKeyMapper _keyMapper = new OpenTkToAwesomiumKeyMapper();
        private readonly OpenGlWindow _inputSource;
        private WebView _webView;
        private Thread _thread;

        protected AwesomiumGui(OpenGlWindow openGlWindow)
        {
            _inputSource = openGlWindow;
        }

        protected void ExecuteJs(string js)
        {
            _webView.ExecuteJavascript(js);
        }

        public bool IsDirty;

        private Frame _frame;
        public Frame Frame
        {
            get
            {
                lock (this)
                {
                    return _frame;
                }
            }
            set
            {
                lock (this)
                {
                    _frame = value;
                }
            }
        }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (_isEnabled == value) return;
                if (value)
                {
                    Enable();
                }
                else
                {
                    Disable();
                }
                _isEnabled = value;
            }
        }

        protected void Run(Action<WebView> action)
        {
            WebCore.QueueWork(_webView, () => action(_webView));
        }

        private void Enable()
        {
            _isEnabled = true;
            _inputSource.Mouse.Move += MouseMove;
            _inputSource.Mouse.ButtonDown += MouseDown;
            _inputSource.Mouse.ButtonUp += MouseUp;
            _inputSource.KeyDown += KeyDown;
            _inputSource.KeyUp += KeyUp;
            _inputSource.KeyPress += KeyPress;
        }

        private void Disable()
        {
            _inputSource.Mouse.Move -= MouseMove;
            _inputSource.Mouse.ButtonDown -= MouseDown;
            _inputSource.Mouse.ButtonUp -= MouseUp;
            _inputSource.KeyDown -= KeyDown;
            _inputSource.KeyUp -= KeyUp;
            _inputSource.KeyPress -= KeyPress;
        }

        public void Start()
        {
            _thread = new Thread(StartWebView);
            _thread.Start();
        }

        public void Stop()
        {
            WebCore.Shutdown();
            _thread.Join();
        }

        private void StartWebView()
        {
            WebCore.Initialize(WebConfig.Default);

            _webView = WebCore.CreateWebView(1024, 768);
            _webView.ConsoleMessage += (sender, args) => Console.WriteLine(args.Message);
            _webView.WebSession.AddDataSource("myhost", new DirectoryDataSource(Path.Combine(Environment.CurrentDirectory, "gui", "views")));
            
            while (!_webView.IsLive) { }

            _webView.IsTransparent = true;
            _webView.Source = new Uri("asset://myhost/index.html");
            
            _webView.LoadingFrameComplete += WebViewOnLoadingFrameComplete;
            _webView.DocumentReady += WebViewOnDocumentReady;
            WebCore.Run();
        }

        private void WebViewOnDocumentReady(object sender, UrlEventArgs urlEventArgs)
        {
            _webView.DocumentReady -= WebViewOnDocumentReady;
            OnDocumentReady();
        }

        protected JSObject CreateJsObject(string objectName)
        {
            return _webView.CreateGlobalJavascriptObject(objectName);
        }

        protected abstract void OnDocumentReady();


        protected void BindTo(JSObject target, object source)
        {
            foreach (var property in source.GetType().GetProperties())
            {
                var value = property.GetValue(source, null);
                target[property.Name] = Convert(value);
            }
        }

        private JSValue Convert(object source)
        {
            var type = source.GetType();
            if (type == typeof(int))
            {
                return new JSValue((int)source);
            }
            if (type == typeof(double))
            {
                return new JSValue((double)source);
            }

            throw new NotImplementedException();
        }


        protected T GetAs<T>(JSObject source) where T : new()
        {
            var settings = new T();
            WebCore.QueueWork(_webView, () =>
            {
                foreach (var property in typeof(T).GetProperties())
                {
                    var value = source[property.Name];
                    var result = System.Convert.ChangeType((string)value, property.PropertyType, CultureInfo.InvariantCulture);
                    property.SetValue(settings, result, null);
                }
            });

            return settings;
        }

        private void WebViewOnLoadingFrameComplete(object sender, FrameEventArgs frameEventArgs)
        {
            var surface = (BitmapSurface)_webView.Surface;
            surface.Updated += (_, __) => EnqeueFrame(surface);

            EnqeueFrame(surface);
        }

        private void EnqeueFrame(BitmapSurface surface)
        {
            var bytes = new byte[surface.RowSpan * surface.Height];
            Marshal.Copy(surface.Buffer, bytes, 0, bytes.Length);
            IsDirty = true;
            Frame = new Frame(bytes, surface.Width, surface.Height);
        }

        private void KeyPress(object sender, KeyPressEventArgs args)
        {
            var webKeyboardEvent = new WebKeyboardEvent
            {
                Type = WebKeyboardEventType.Char,
                Text = new String(new[] { args.KeyChar, (char)0, (char)0, (char)0 })
            };

            WebCore.QueueWork(_webView, () => _webView.InjectKeyboardEvent(webKeyboardEvent));
        }

        private void KeyDown(object sender, KeyboardKeyEventArgs args)
        {
            InjectKey(args, WebKeyboardEventType.KeyDown);
        }

        private void KeyUp(object sender, KeyboardKeyEventArgs args)
        {
            InjectKey(args, WebKeyboardEventType.KeyUp);
        }

        private void MouseMove(object sender, MouseMoveEventArgs args)
        {
            WebCore.QueueWork(_webView, () => _webView.InjectMouseMove(args.X, args.Y));
        }

        private void InjectKey(KeyboardKeyEventArgs e, WebKeyboardEventType webKeyboardEventType)
        {
            var virtualKeyCode = _keyMapper.Map(e.Key);

            var webKeyboardEvent = new WebKeyboardEvent
            {
                Type = webKeyboardEventType,
                VirtualKeyCode = virtualKeyCode,
                Modifiers = ConvertModifiers(e.Modifiers),
                KeyIdentifier = Utilities.GetKeyIdentifierFromVirtualKeyCode(virtualKeyCode)
            };

            WebCore.QueueWork(_webView, () => _webView.InjectKeyboardEvent(webKeyboardEvent));
        }

        private static Modifiers ConvertModifiers(KeyModifiers keyModifiers)
        {
            Modifiers modifiers = 0;

            if (keyModifiers == KeyModifiers.Alt)
                modifiers |= Modifiers.AltKey;
            if (keyModifiers == KeyModifiers.Control)
                modifiers |= Modifiers.ControlKey;
            if (keyModifiers == KeyModifiers.Shift)
                modifiers |= Modifiers.ShiftKey;

            return modifiers;
        }

        private void MouseDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            WebCore.QueueWork(_webView, () =>
            {
                if (IsCoordinateInGui(mouseButtonEventArgs.X, mouseButtonEventArgs.Y))
                {
                    _webView.InjectMouseDown(Awesomium.Core.MouseButton.Left);
                    _webView.FocusView();
                }
                else
                {
                    _webView.UnfocusView();
                }

            });
        }

        public bool IsCoordinateInGui(int x, int y)
        {
            var surface = ((BitmapSurface)_webView.Surface);
            var alphaAtPoint = surface.GetAlphaAtPoint(x, y);
            return alphaAtPoint != 0;
        }

        private void MouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            WebCore.QueueWork(_webView, () => _webView.InjectMouseUp(Awesomium.Core.MouseButton.Left));
        }

        public void Resize(int width, int height)
        {
            WebCore.QueueWork(_webView, () => _webView.Resize(width, height));
        }
    }
}