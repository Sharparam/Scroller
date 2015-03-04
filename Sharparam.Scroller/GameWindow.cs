namespace Sharparam.Scroller
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;

    using SFML.Graphics;
    using SFML.Window;

    public class GameWindow : IStateManager
    {
        private readonly Stack<IState> _states;

        private readonly Stopwatch _updateTimer;

        private Thread _renderThread;

        private TimeSpan _timerDelta;

        public GameWindow(uint width, uint height, string title, Styles style = Styles.Default)
        {
            _states = new Stack<IState>();
            ClearColor = Color.Black;
            Window = new RenderWindow(new VideoMode(width, height), title, style);
            Window.SetActive(false);

            Window.Closed += (sender, args) =>
            {
                if (StateCount > 0)
                    CurrentState.OnClose();
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

        public IState Peek()
        {
            // Same reasoning as with Pop
            if (StateCount < 1)
                throw new InvalidOperationException("Cannot Peek state when no states exist.");
            return _states.Peek();
        }

        public IState Pop()
        {
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
            _renderThread = new Thread(Render) { Name = "SFMLRender" };
            _renderThread.Start();

            while (Window.IsOpen())
            {
                Window.DispatchEvents();
                var elapsed = _updateTimer.Elapsed;
                Update(elapsed + _timerDelta);
                _timerDelta = _updateTimer.Elapsed - elapsed;
                _updateTimer.Restart();
            }

            _renderThread.Join();
        }

        protected virtual void Draw(RenderWindow window)
        {
            Window.Clear(ClearColor);
            if (StateCount > 0)
                CurrentState.Draw(window);
        }

        protected virtual void Update(TimeSpan elapsed)
        {
            if (StateCount > 0)
                CurrentState.Update(elapsed);
        }

        private void Render(object state)
        {
            while (Window.IsOpen())
            {
                Draw(Window);
                Window.Display();
            }
        }
    }
}
