namespace Sharparam.Scroller
{
    using System;
    using System.Threading;

    using log4net;
    using log4net.Config;
    using log4net.Core;

    using SFML.Window;

    using Sharparam.Scroller.States;

    public static class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));

        public static void Main(string[] args)
        {
            try
            {
                Thread.CurrentThread.Name = "MAIN";
            }
            catch (InvalidOperationException)
            {
                // Name was already set, do nothing.
            }

            XmlConfigurator.Configure();
#if DEBUG
            ((log4net.Repository.Hierarchy.Hierarchy)LogManager.GetRepository()).Root.Level = Level.Debug;
            ((log4net.Repository.Hierarchy.Hierarchy)LogManager.GetRepository()).RaiseConfigurationChanged(
                EventArgs.Empty);
#endif

            Log.Info("Sharparam.Scroller is starting.");

            Log.Debug("Creating window...");

            var window = new GameWindow(800, 600, "Scroller", Styles.Close | Styles.Titlebar);
            window.Push(new TestState(window));
            window.Run();

            Log.Info("Sharparam.Scroller is shutting down.");
        }
    }
}
