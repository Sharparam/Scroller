namespace Sharparam.Scroller
{
    using System;

    using SFML.Graphics;
    using SFML.Window;

    public class TestState : IState
    {
        private readonly GameWindow _window;

        private Font _font;

        private Text _text;

        private bool _loaded;

        private int _frameCount;

        private int _fps;

        private static readonly TimeSpan FpsUpdateDelay = TimeSpan.FromSeconds(1);

        private TimeSpan _fpsElapsed = TimeSpan.Zero;

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

        public void Draw(RenderWindow window)
        {
            _frameCount++;

            lock (_text)
            {
                window.Draw(_text);   
            }
        }

        public void Disable()
        {
            Console.WriteLine("Disabled!");
        }

        public void Enable()
        {
            Console.WriteLine("Enabled!");
            if (_loaded)
                return;
            Console.WriteLine("Loading font...");
            _font = new Font(@"res\fonts\BebasNeue.otf");
            _text = new Text("Hello, World!", _font) { Color = Color.White };
            _loaded = true;
        }

        public void OnClose()
        {
            Console.WriteLine("OnClose!");
            _window.Close();
        }

        public void OnKeyPressed(KeyEventArgs args)
        {
            Console.WriteLine("OnKeyPressed: {0}", args.Code);
        }

        public void OnKeyReleased(KeyEventArgs args)
        {
            Console.WriteLine("OnKeyReleased: {0}", args.Code);
        }

        public void OnMouseButtonPressed(MouseButtonEventArgs args)
        {
            Console.WriteLine("OnMouseButtonPressed: {0} ({1}, {2})", args.Button, args.X, args.Y);
        }

        public void OnMouseButtonReleased(MouseButtonEventArgs args)
        {
            Console.WriteLine("OnMouseButtonReleased: {0} ({1}, {2})", args.Button, args.X, args.Y);
        }

        public void OnMouseMoved(MouseMoveEventArgs args)
        {
            //Console.WriteLine("OnMouseMoved: {0}, {1}", args.X, args.Y);
        }

        public void OnMouseWheelMoved(MouseWheelEventArgs args)
        {
            Console.WriteLine("OnMouseWheelMoved: {0}", args.Delta);
        }

        public void OnMouseLeft()
        {
            Console.WriteLine("OnMouseLeft!");
        }

        public void OnMouseEntered()
        {
            Console.WriteLine("OnMouseEntered!");
        }

        public void OnGainedFocus()
        {
            Console.WriteLine("OnGainedFocus!");
        }

        public void OnLostFocus()
        {
            Console.WriteLine("OnLostFocus!");
        }
    }
}
