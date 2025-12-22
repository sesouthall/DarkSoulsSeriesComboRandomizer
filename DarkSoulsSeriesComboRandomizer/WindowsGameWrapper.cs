using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DarkSoulsSeriesComboRandomizer
{
    public abstract class WindowsGameWrapper : IGameWrapper, IDisposable
    {
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

        private Process gameProcess;
        private bool disposedValue;

        public abstract event EventFlagReactor? OnModEventSet;
        public abstract event ItemReactor? OnModItemPickUp;

        public WindowsGameWrapper(string exePath)
        {
            gameProcess = new Process()
            {
                StartInfo = new ProcessStartInfo(exePath)
                {
                    CreateNoWindow = false,
                    UseShellExecute = true,
                    WorkingDirectory = Path.GetDirectoryName(exePath)
                }
            };
        }

        public virtual void Start()
        {
            gameProcess.Start();
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
            ShowWindow(gameProcess.MainWindowHandle, 7);
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

        public void Resume()
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

            ShowWindow(gameProcess.MainWindowHandle, 3);
            SetForegroundWindow(gameProcess.MainWindowHandle);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    gameProcess.Close();
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