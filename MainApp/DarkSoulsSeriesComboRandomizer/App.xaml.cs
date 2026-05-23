using System.Windows;

namespace DarkSoulsSeriesComboRandomizer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(DebugExceptionHandler);
        }

        static void DebugExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            Serilog.Log.Fatal($"Unhandled exception: {e.ExceptionObject}");
            Serilog.Log.CloseAndFlush();
        }
    }

}
