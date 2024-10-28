using Avalonia;
using System;

namespace RöstVävare
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args) => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<Views.App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace();
    }
}
