using DarkSoulsSeriesComboRandomizer.DarkSouls2SotFS;
using DarkSoulsSeriesComboRandomizer.DarkSouls3;
using DarkSoulsSeriesComboRandomizer.DarkSoulsRemastered;

namespace DarkSoulsSeriesComboRandomizer
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var hook = new DS3Hook(5000, 5000);
            hook.Start();

            while (!hook.Hooked)
            {
                Thread.Sleep(1000);
            }

            hook.ReadEventFlag(6104);
            hook.WriteEventFlag(6104, true);
            hook.ReadEventFlag(6104);
            hook.Stop();

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}