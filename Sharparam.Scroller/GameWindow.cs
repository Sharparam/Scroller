namespace Sharparam.Scroller
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;

    using log4net;

    using SFML.Graphics;
    using SFML.Window;

    using Sharparam.Scroller.States;

    public class GameWindow : IStateManager
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(GameWindow));

        private readonly Stack<IState> _states;

        private readonly Stopwatch _updateTimer;

        private Thread _renderThread;

        private TimeSpan _timerDelta;

        public GameWindow(uint width, uint height, string title, Styles style = Styles.Default)
        {
            Log.Debug("Creating state stack");
            _states = new Stack<IState>();
            ClearColor = Color.Black;
            Log.Debug("Initializing SFML render window");
            Window = new RenderWindow(new VideoMode(width, height), title, style);
            Window.SetActive(false);
            Window.SetFramerateLimit(60);

            Log.Debug("Setting up window events.");

            Window.Closed += (sender, args) =>
            {
                Log.Debug("Window close requested by user! (Window.Closed)");
                if (StateCount > 0)
                    CurrentState.OnClose();
                else // Fallback when no states
                    Close();
            };

            Window.KeyPressed += (sender, args) =>
            {
                if (StateCount > 0)
                    CurrentState.OnKeyPressed(args);
            };

            Window.KeyReleased += (sender, args) =>
            {
                if (StateCount > 0)
                    CurrentState.OnKeyReleased(args);
            };

            Window.MouseButtonPressed += (sender, args) =>
            {
                if (StateCount > 0)
                    CurrentState.OnMouseButtonPressed(args);
            };

            Window.MouseButtonReleased += (sender, args) =>
            {
                if (StateCount > 0)
                    CurrentState.OnMouseButtonReleased(args);
            };

            Window.MouseMoved += (sender, args) =>
            {
                if (StateCount > 0)
                    CurrentState.OnMouseMoved(args);
            };

            Window.MouseWheelMoved += (sender, args) =>
            {
                if (StateCount > 0)
                    CurrentState.OnMouseWheelMoved(args);
            };

            Window.MouseLeft += (sender, args) =>
            {
                if (StateCount > 0)
                    CurrentState.OnMouseLeft();
            };

            Window.MouseEntered += (sender, args) =>
            {
                if (StateCount > 0)
                    CurrentState.OnMouseEntered();
            };

            Window.GainedFocus += (sender, args) =>
            {
                if (StateCount > 0)
                    CurrentState.OnGainedFocus();
            };

            Window.LostFocus += (sender, args) =>
            {
                if (StateCount > 0)
                    CurrentState.OnLostFocus();
            };

            Log.Debug("Initializing timer data");
            _timerDelta = TimeSpan.Zero;
            _updateTimer = new Stopwatch();
            _updateTimer.Start();
        }

        public Color ClearColor { get; set; }

        public IState CurrentState
        {
            get
            {
                return Peek();
            }
        }

        public int StateCount
        {
            get
            {
                return _states.Count;
            }
        }

        public RenderWindow Window { get; private set; }

        // Convenience method
        public void Close()
        {
            Window.Close();
        }

        public IState Peek()
        {
            // Same reasoning as with Pop
            if (StateCount < 1)
                throw new InvalidOperationException("Cannot Peek state when no states exist.");
            return _states.Peek();
        }

        public IState Pop()
        {
            Log.Debug("Popping current state.");
            // For Pop, it is reasonable to assume the dev would check before popping.
            if (StateCount < 1)
                throw new InvalidOperationException("Cannot Pop states when no states exist.");
            var state = _states.Pop();
            state.Disable();
            Peek().Enable();
            return state;
        }

        public void Push(IState state)
        {
            Log.Debug("Pushing new state onto stack");
            if (StateCount > 0)
                Peek().Disable();
            _states.Push(state);
            state.Enable();
        }

        /// <summary>
        /// Replaces the current state with the provided one and returns the replaced <see cref="IState" />.
        /// If the current state stack is empty, behaves like <see cref="Push" /> and returns <c>null</c>.
        /// </summary>
        /// <param name="state">The state to replace the current state with.</param>
        /// <returns>The replaced state, or <c>null</c> if there was none.</returns>
        public IState Replace(IState state)
        {
            Log.Debug("Replacing current state with new");
            // We don't use our own Pop method as it will cause an Enable
            // and immediate re-disabling of the underlying state
            IState old = null;
            if (StateCount > 0)
            {
                old = _states.Pop();
                old.Disable();
            }
            // For the push it's fine though, as it was already disabled.
            Push(state);
            return old;
        }

        /// <summary>
        /// Starts event and rendering loops.
        /// </summary>
        public void Run()
        {
            Log.Debug("ENTER: Run()");
            Log.Debug("Creating and starting render thread as SFMLRender.");
            _renderThread = new Thread(Render) { Name = "SFMLRender" };
            _renderThread.Start();

            Log.Debug("Starting event + update loop.");

            while (Window.IsOpen())
            {
                Window.DispatchEvents();
                var elapsed = _updateTimer.Elapsed;
                Update(elapsed + _timerDelta);
                _timerDelta = _updateTimer.Elapsed - elapsed;
                _updateTimer.Restart();
            }

            Log.Debug("Window has closed, joining render thread.");

            _renderThread.Join();
        }

        protected virtual void Draw(RenderTarget target)
        {
            Window.Clear(ClearColor);
            if (StateCount > 0)
                target.Draw(CurrentState);
        }

        protected virtual void Update(TimeSpan elapsed)
        {
            if (StateCount > 0)
                CurrentState.Update(elapsed);
        }

        private void Render(object state)
        {
            Log.Debug("This is render thread, starting.");
            while (Window.IsOpen())
            {
                Draw(Window);
                Window.Display();
            }
            Log.Debug("Window closed, render thread is exiting.");
        }
    }
}
