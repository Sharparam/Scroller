namespace Sharparam.Scroller.States
{
    public interface IStateManager
    {
        int StateCount { get; }

        IState CurrentState { get; }

        void Push(IState state);

        IState Replace(IState state);

        IState Pop();

        IState Peek();
    }
}
