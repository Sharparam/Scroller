namespace Sharparam.Scroller
{
    using System;

    using SFML.Window;

    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Creating window...");
            var window = new GameWindow(800, 600, "Scroller", Styles.Close | Styles.Titlebar);
            window.Push(new TestState(window));
            window.Run();
        }
    }
}
