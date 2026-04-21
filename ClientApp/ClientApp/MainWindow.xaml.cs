using System;

using System.Diagnostics;

using System.Net.Sockets;

using System.Runtime.InteropServices;

using System.Text;

using System.Threading;

using System.Windows;

using System.Windows.Threading;
 
namespace ClientApp

{

    public partial class MainWindow : Window

    {

        TcpClient client;
 
        bool isLocked = false;

        int remainingTime = 0;
 
        DispatcherTimer timer;
 
        // ===== KEYBOARD HOOK =====

        private static IntPtr keyboardHookID = IntPtr.Zero;

        private delegate IntPtr LowLevelProc(int nCode, IntPtr wParam, IntPtr lParam);

        private LowLevelProc keyboardProc;
 
        private const int WH_KEYBOARD_LL = 13;
 
        // ===== MOUSE HOOK =====

        private static IntPtr mouseHookID = IntPtr.Zero;

        private LowLevelProc mouseProc;
 
        private const int WH_MOUSE_LL = 14;
 
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
 
            // keyboard

            keyboardProc = KeyboardCallback;

            keyboardHookID = SetHook(keyboardProc, WH_KEYBOARD_LL);
 
            // mouse

            mouseProc = MouseCallback;

            mouseHookID = SetHook(mouseProc, WH_MOUSE_LL);
 
            // timer

            timer = new DispatcherTimer();

            timer.Interval = TimeSpan.FromSeconds(1);

            timer.Tick += Timer_Tick;

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

            remainingTime = seconds;

            TimerText.Text = $"Pozostały czas: {remainingTime}s";

            timer.Start();

        }
 
        private void Timer_Tick(object sender, EventArgs e)

        {

            remainingTime = Math.Max(0, remainingTime - 1);
 
            if (remainingTime > 0)

                TimerText.Text = $"Pozostały czas: {remainingTime}s";

            else

                TimerText.Text = "";
 
            if (remainingTime <= 0)

            {

                timer.Stop();

                Lock();

            }

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
 
            timer.Stop();

        }
 
        // ===== KEYBOARD BLOCK =====

        private IntPtr KeyboardCallback(int nCode, IntPtr wParam, IntPtr lParam)

        {

            if (isLocked)

                return (IntPtr)1;
 
            return CallNextHookEx(keyboardHookID, nCode, wParam, lParam);

        }
 
        // ===== MOUSE BLOCK =====

        private IntPtr MouseCallback(int nCode, IntPtr wParam, IntPtr lParam)

        {

            if (isLocked)

                return (IntPtr)1;
 
            return CallNextHookEx(mouseHookID, nCode, wParam, lParam);

        }
 
        private IntPtr SetHook(LowLevelProc proc, int hookType)

        {

            using (Process curProcess = Process.GetCurrentProcess())

            using (ProcessModule curModule = curProcess.MainModule)

            {

                return SetWindowsHookEx(hookType, proc,

                    GetModuleHandle(curModule.ModuleName), 0);

            }

        }
 
        protected override void OnClosed(EventArgs e)

        {

            UnhookWindowsHookEx(keyboardHookID);

            UnhookWindowsHookEx(mouseHookID);

            base.OnClosed(e);

        }

    }

}
 
