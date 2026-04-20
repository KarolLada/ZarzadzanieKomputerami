using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

namespace ClientApp
{
    public partial class MainWindow : Window
    {
        TcpClient client;

        public MainWindow()
        {
            InitializeComponent();
            Connect();
        }

        private void Connect()
        {
            client = new TcpClient("10.10.10.102", 5000);

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
                        BlockOverlay.Visibility = Visibility.Visible;
                    else if (msg == "UNBLOCK")
                        BlockOverlay.Visibility = Visibility.Collapsed;
                });
            }
        }
    }
}