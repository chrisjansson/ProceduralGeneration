using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using Awesomium.Core;
using CjClutter.OpenGl.Noise;
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

    public class AwesomiumGui
    {
        private readonly OpenTkToAwesomiumKeyMapper _keyMapper = new OpenTkToAwesomiumKeyMapper();
        private readonly OpenGlWindow _inputSource;
        private WebView _webView;

        
        private Thread _thread;
        private JSObject _viewModel;

        public event Action<FractalBrownianMotionSettings> SettingsChanged;

        public AwesomiumGui(OpenGlWindow openGlWindow)
        {
            _inputSource = openGlWindow;
            SettingsChanged += _ => { };
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
            while (!_webView.IsLive) { }

            _webView.IsTransparent = true;
            _webView.LoadHTML(Source);
            _webView.LoadingFrameComplete += WebViewOnLoadingFrameComplete;
            _webView.DocumentReady += WebViewOnDocumentReady;
            WebCore.Run();
        }

        private void WebViewOnDocumentReady(object sender, UrlEventArgs urlEventArgs)
        {
            _webView.DocumentReady -= WebViewOnDocumentReady;

            _viewModel = _webView.ExecuteJavascriptWithResult("viewModel");
            dynamic viewModel = _viewModel;
            viewModel.octaves = FractalBrownianMotionSettings.Default.Octaves;
            viewModel.frequency = FractalBrownianMotionSettings.Default.Frequency;
            viewModel.amplitude = FractalBrownianMotionSettings.Default.Amplitude;
            viewModel.apply = (JavascriptAsynchMethodEventHandler) Apply;
            _webView.ExecuteJavascript("echo();");
        }

        private void Apply(object sender, JavascriptMethodEventArgs e)
        {
            dynamic viewModel = _viewModel;
            var octaves = int.Parse(viewModel.octaves, CultureInfo.InvariantCulture);
            var amplitude = double.Parse(viewModel.amplitude, CultureInfo.InvariantCulture);
            var frequency = double.Parse(viewModel.frequency, CultureInfo.InvariantCulture);

            var fractalBrownianMotionSettings = new FractalBrownianMotionSettings(octaves, amplitude, frequency);
            SettingsChanged(fractalBrownianMotionSettings);
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
                _webView.InjectMouseDown(Awesomium.Core.MouseButton.Left);
                _webView.FocusView();
            });
        }

        private void MouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            WebCore.QueueWork(_webView, () => _webView.InjectMouseUp(Awesomium.Core.MouseButton.Left));
        }

        public void Resize(int width, int height)
        {
            WebCore.QueueWork(_webView, () => _webView.Resize(width, height));
        }

        private const string Source = @"
<html>
    <head>
        <script type='text/javascript'>
            var viewModel = {
                frequency : 1.0,
                amplitude: 2.0,
                octaves: 3.0,
            };

            var echo = function() {
                document.getElementById('frequency').value = viewModel.frequency;
                document.getElementById('amplitude').value = viewModel.amplitude;
                document.getElementById('octaves').value = viewModel.octaves;
            }

            var applyValues = function() {
                viewModel.frequency = document.getElementById('frequency').value;
                viewModel.amplitude = document.getElementById('amplitude').value;
                viewModel.octaves = document.getElementById('octaves').value;
                viewModel.apply();
            };         
        </script>
    </head>
    <body style='margin: 0px'>
        <div style='width: 100%;background: red'>
            Octaves: <input id='octaves' /><br>
            Amplitude: <input id='amplitude' /><br>
            Frequency: <input id='frequency' /><br>
            <input type='button' value='Apply' onclick='applyValues()'></input>
        </div>
    </body>
</html>";
    }
}