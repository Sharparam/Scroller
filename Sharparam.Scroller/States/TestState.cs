namespace Sharparam.Scroller.States
{
    using System;

    using log4net;

    using SFML.Graphics;
    using SFML.Window;

    using Sharparam.Scroller.Mapping;

    public class TestState : IState
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(TestState));

        private readonly GameWindow _window;

        private Font _font;

        private Text _text;

        private bool _loaded;

        private int _frameCount;

        private int _fps;

        private Map _map;

        private static readonly TimeSpan FpsUpdateDelay = TimeSpan.FromSeconds(1);

        private TimeSpan _fpsElapsed = TimeSpan.Zero;

        private bool _mouseDown;

        private Vector2f _lastPos;

        public TestState(GameWindow window)
        {
            _window = window;
        }

        public void Update(TimeSpan elapsed)
        {
            _fpsElapsed += elapsed;
            if (_fpsElapsed >= FpsUpdateDelay)
            {
                _fps = _frameCount;
                _frameCount = 0;
                _fpsElapsed -= FpsUpdateDelay;
            }

            lock (_text)
            {
                _text.DisplayedString = string.Format("Frames: {0}, FPS: {1}", _frameCount, _fps);   
            }
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            _frameCount++;

            target.Draw(_map);

            lock (_text)
            {
                target.Draw(_text);   
            }
        }

        public void Disable()
        {
            Console.WriteLine("Disabled!");
        }

        public void Enable()
        {
            Log.Debug("Enabled!");
            if (_loaded)
                return;
            Log.Debug("Loading font...");
            _font = ResourceManager.LoadFont("BebasNeue.otf"); ////new Font(@"res\fonts\BebasNeue.otf");
            _text = new Text("Hello, World!", _font) { Color = Color.White };
            Log.Debug("Loading test map...");
            _map = ResourceManager.LoadMap("test"); ////new Map(@"res\maps\test.tmx");
            _loaded = true;
        }

        public void OnClose()
        {
            Log.Debug("OnClose!");
            _window.Close();
        }

        public void OnKeyPressed(KeyEventArgs args)
        {
            //Log.DebugFormat("OnKeyPressed: {0}", args.Code);
        }

        public void OnKeyReleased(KeyEventArgs args)
        {
            //Log.DebugFormat("OnKeyReleased: {0}", args.Code);
        }

        public void OnMouseButtonPressed(MouseButtonEventArgs args)
        {
            //Log.DebugFormat("OnMouseButtonPressed: {0} ({1}, {2})", args.Button, args.X, args.Y);
            if (args.Button == Mouse.Button.Left)
                _mouseDown = true;
        }

        public void OnMouseButtonReleased(MouseButtonEventArgs args)
        {
            //Log.DebugFormat("OnMouseButtonReleased: {0} ({1}, {2})", args.Button, args.X, args.Y);
            if (args.Button == Mouse.Button.Left)
                _mouseDown = false;
        }

        public void OnMouseMoved(MouseMoveEventArgs args)
        {
            var newPos = new Vector2f(args.X, args.Y);
            if (_mouseDown)
            {
                var moveDelta = _lastPos - newPos;
                _map.View.Move(moveDelta);
            }
            _lastPos = newPos;
        }

        public void OnMouseWheelMoved(MouseWheelEventArgs args)
        {
            _map.View.Zoom(args.Delta == 1 ? 0.5f : 1.5f);
        }

        public void OnMouseLeft()
        {
            //Log.Debug("OnMouseLeft!");
        }

        public void OnMouseEntered()
        {
            //Log.Debug("OnMouseEntered!");
        }

        public void OnGainedFocus()
        {
            //Log.Debug("OnGainedFocus!");
        }

        public void OnLostFocus()
        {
            //Log.Debug("OnLostFocus!");
        }
    }
}
