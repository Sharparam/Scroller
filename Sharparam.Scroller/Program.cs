namespace Sharparam.Scroller
{
    using SFML.Window;

    public static class Program
    {
        public static void Main(string[] args)
        {
            var window = new GameWindow(800, 600, "Scroller", Styles.Close | Styles.Titlebar);
            window.Run();
        }
    }
}
