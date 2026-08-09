using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Linq;
using System.Threading;

namespace DarkSoulsSeriesComboRandomizer
{
    public abstract class WindowsGameWrapper : IDisposable
    {
        // How long to Sleep in Pause() before suspending threads. Default kept at original 500ms.
        public int PauseSleepMs { get; set; } = 500;

        // How long to Sleep in Resume() after resuming threads but before restoring/maximizing the window.
        public int ResumeAfterThreadsSleepMs { get; set; } = 0;

        [Flags]
        public enum ThreadAccess : int
        {
            TERMINATE = (0x0001),
            SUSPEND_RESUME = (0x0002),
            GET_CONTEXT = (0x0008),
            SET_CONTEXT = (0x0010),
            SET_INFORMATION = (0x0020),
            QUERY_INFORMATION = (0x0040),
            SET_THREAD_TOKEN = (0x0080),
            IMPERSONATE = (0x0100),
            DIRECT_IMPERSONATION = (0x0200)
        }

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);
        [DllImport("kernel32.dll")]
        static extern uint SuspendThread(IntPtr hThread);
        [DllImport("kernel32.dll")]
        static extern int ResumeThread(IntPtr hThread);
        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool CloseHandle(IntPtr handle);
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("User32.dll")]
        public static extern bool ShowWindow(IntPtr handle, int nCmdShow);

        public event EventHandler? Exited;

        private Process gameProcess;
        private bool processRunning = false;
        private bool disposedValue;

        public WindowsGameWrapper(string exePath, string args = "")
        {
            gameProcess = new Process()
            {
                StartInfo = new ProcessStartInfo(exePath, args)
                {
                    WorkingDirectory = Path.GetDirectoryName(exePath)
                }
            };
        }

        public virtual async Task Start()
        {
            gameProcess.Start();
            gameProcess.Exited += OnExited;
            processRunning = true;
        }

        private void OnExited(object? sender, EventArgs e)
        {
            Exited?.Invoke(this, e);
        }

        public async Task WaitForShutdown()
        {
            if (!processRunning)
            {
                return;
            }

            Resume();
            try
            {
                await gameProcess.WaitForExitAsync();
            }
            catch (InvalidOperationException)
            {
                // The game isn't running
                processRunning = false;
            }
        }

        protected void RefreshProcess(string MainWindowTitle)
        {
            if (gameProcess.HasExited)
            {
                gameProcess.Dispose();
                gameProcess = Process.GetProcessesByName(MainWindowTitle).Single();
            }
        }

        public void Pause()
        {
            if (!processRunning)
            {
                return;
            }

            try
            {
                ShowWindow(gameProcess.MainWindowHandle, 7);
                Thread.Sleep(PauseSleepMs);
                foreach (ProcessThread pT in gameProcess.Threads)
                {
                    IntPtr pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);

                    if (pOpenThread == IntPtr.Zero)
                    {
                        continue;
                    }

                    SuspendThread(pOpenThread);

                    CloseHandle(pOpenThread);
                }
            }
            catch (InvalidOperationException)
            {
                // The game isn't running.
                processRunning = false;
            }
        }

        public void Resume()
        {
            if (!processRunning)
            {
                return;
            }

            try
            {
                foreach (ProcessThread pT in gameProcess.Threads)
                {
                    IntPtr pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);

                    if (pOpenThread == IntPtr.Zero)
                    {
                        continue;
                    }

                    var suspendCount = 0;
                    do
                    {
                        suspendCount = ResumeThread(pOpenThread);
                    } while (suspendCount > 0);

                    CloseHandle(pOpenThread);
                }

                // Allow a short pause after threads have been resumed before restoring/maximizing
                // the window to improve reliability when switching between games.
                if (ResumeAfterThreadsSleepMs > 0)
                    Thread.Sleep(ResumeAfterThreadsSleepMs);

                ShowWindow(gameProcess.MainWindowHandle, 3);
                SetForegroundWindow(gameProcess.MainWindowHandle);
            }
            catch (InvalidOperationException)
            {
                // The game isn't running
                processRunning = false;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (processRunning)
                    {
                        Resume();
                    }
                    gameProcess.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}