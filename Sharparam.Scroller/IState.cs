namespace Sharparam.Scroller
{
    using System;

    using SFML.Graphics;
    using SFML.Window;

    public interface IState
    {
        void Update(TimeSpan elapsed);

        void Draw(RenderWindow window);

        void Disable();

        void Enable();

        void OnClose();

        void OnKeyPressed(KeyEventArgs args);

        void OnKeyReleased(KeyEventArgs args);

        void OnMouseButtonPressed(MouseButtonEventArgs args);

        void OnMouseButtonReleased(MouseButtonEventArgs args);

        void OnMouseMoved(MouseMoveEventArgs args);

        void OnMouseWheelMoved(MouseWheelEventArgs args);

        void OnMouseLeft();

        void OnMouseEntered();

        void OnGainedFocus();

        void OnLostFocus();
    }
}
