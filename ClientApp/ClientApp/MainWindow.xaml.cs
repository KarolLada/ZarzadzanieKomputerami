
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows;

namespace ClientApp
{
    public partial class MainWindow : Window
    {
        TcpClient client;

        bool isLocked = false;

        int remainingTime = 0;
        System.Timers.Timer lockTimer;
        System.Timers.Timer uiTimer;

        // keyboard hook
        private static IntPtr hookID = IntPtr.Zero;
        private delegate IntPtr LowLevelProc(int nCode, IntPtr wParam, IntPtr lParam);
        private LowLevelProc proc;

        private const int WH_KEYBOARD_LL = 13;

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        public MainWindow()
        {
            InitializeComponent();
            Connect();

            proc = HookCallback;
            hookID = SetHook(proc);
        }

        private void Connect()
        {
            client = new TcpClient("127.0.0.1", 5000);

            Thread t = new Thread(Listen);
            t.IsBackground = true;
            t.Start();
        }

        private void Listen()
        {
            var stream = client.GetStream();
            byte[] buffer = new byte[1024];

            while (true)
            {
                int bytes = stream.Read(buffer, 0, buffer.Length);
                string msg = Encoding.UTF8.GetString(buffer, 0, bytes);

                Dispatcher.Invoke(() =>
                {
                    if (msg == "BLOCK")
                        Lock();
                    else if (msg == "UNBLOCK")
                        Unlock();
                    else if (msg.StartsWith("TIMER:"))
                        StartTimer(int.Parse(msg.Split(':')[1]));
                });
            }
        }

        private void StartTimer(int seconds)
        {
            lockTimer?.Stop();
            uiTimer?.Stop();

            remainingTime = seconds;

            lockTimer = new System.Timers.Timer(1000);
            lockTimer.Elapsed += (s, e) =>
            {
                remainingTime--;

                if (remainingTime <= 0)
                {
                    lockTimer.Stop();
                    Dispatcher.Invoke(Lock);
                }
            };
            lockTimer.Start();

            uiTimer = new System.Timers.Timer(1000);
            uiTimer.Elapsed += (s, e) =>
            {
                Dispatcher.Invoke(() =>
                {
                    TimerText.Text = $"Pozostały czas: {remainingTime}s";
                });
            };
            uiTimer.Start();
        }

        private void Lock()
        {
            isLocked = true;
            BlockOverlay.Visibility = Visibility.Visible;
        }

        private void Unlock()
        {
            isLocked = false;
            BlockOverlay.Visibility = Visibility.Collapsed;

            remainingTime = 0;
            TimerText.Text = "";

            lockTimer?.Stop();
            uiTimer?.Stop();
        }

        // ===== KEYBOARD BLOCK =====

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (isLocked)
                return (IntPtr)1;

            return CallNextHookEx(hookID, nCode, wParam, lParam);
        }

        private IntPtr SetHook(LowLevelProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            UnhookWindowsHookEx(hookID);
            base.OnClosed(e);
        }
    }
}

//
