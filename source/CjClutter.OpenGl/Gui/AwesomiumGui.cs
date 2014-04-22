using System;
using System.Collections.Concurrent;
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
        private WebView _webView;
        private readonly OpenTkToAwesomiumKeyMapper _keyMapper = new OpenTkToAwesomiumKeyMapper();
        public readonly ConcurrentQueue<Frame> _frames = new ConcurrentQueue<Frame>();
        private Thread _thread;
        private JSObject _viewModel;

        public event Action<FractalBrownianMotionSettings> SettingsChanged;

        public AwesomiumGui()
        {
            SettingsChanged += _ => { };
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

            //_viewModel = _webView.CreateGlobalJavascriptObject("viewModel");
            _viewModel = _webView.ExecuteJavascriptWithResult("viewModel");
            _viewModel.Bind("apply", false, (_, a) =>
            {
                var octaves = _viewModel["octaves"].IsInteger ? int.Parse(_viewModel["octaves"]) : 0;
                var amplitude = _viewModel["octaves"].IsInteger ? double.Parse(_viewModel["octaves"]) : 0;
                var frequency = _viewModel["octaves"].IsInteger ? double.Parse(_viewModel["octaves"]) : 0;

                var fractalBrownianMotionSettings = new FractalBrownianMotionSettings(octaves, amplitude, frequency);
                SettingsChanged(fractalBrownianMotionSettings);
            });
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
            _frames.Enqueue(new Frame(bytes, surface.Width, surface.Height));
        }

        public void KeyPress(KeyPressEventArgs args)
        {
            var webKeyboardEvent = new WebKeyboardEvent
            {
                Type = WebKeyboardEventType.Char,
                Text = new String(new[] { args.KeyChar, (char)0, (char)0, (char)0 })
            };

            WebCore.QueueWork(_webView, () => _webView.InjectKeyboardEvent(webKeyboardEvent));
        }

        public void KeyDown(KeyboardKeyEventArgs args)
        {
            InjectKey(args, WebKeyboardEventType.KeyDown);
        }

        public void KeyUp(KeyboardKeyEventArgs args)
        {
            InjectKey(args, WebKeyboardEventType.KeyUp);
        }

        public void MouseMove(MouseMoveEventArgs args)
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

        public void MouseDown(MouseButtonEventArgs args)
        {
            WebCore.QueueWork(_webView, () =>
            {
                _webView.InjectMouseDown(Awesomium.Core.MouseButton.Left);
                _webView.FocusView();
            });
        }

        public void MouseUp(MouseButtonEventArgs args)
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
            var viewModel = {};
            viewModel.frequency = 1;
            viewModel.amplitude = 2;
            viewModel.octaves = 3;

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