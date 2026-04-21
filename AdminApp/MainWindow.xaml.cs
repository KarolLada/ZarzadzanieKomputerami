using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

namespace AdminApp
{
    public partial class MainWindow : Window
    {
        TcpListener server;
        ObservableCollection<TcpClient> clients = new ObservableCollection<TcpClient>();

        bool serverStarted = false;

        public MainWindow()
        {
            InitializeComponent();
            ClientsList.ItemsSource = clients;
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (PasswordBox.Password == "admin")
            {
                if (!serverStarted)
                {
                    StartServer();
                    serverStarted = true;
                }

                LoginPanel.Visibility = Visibility.Collapsed;
                AdminPanel.Visibility = Visibility.Visible;

                MessageBox.Show("Zalogowano!");
            }
            else
            {
                MessageBox.Show("Złe hasło");
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            AdminPanel.Visibility = Visibility.Collapsed;
            LoginPanel.Visibility = Visibility.Visible;

            PasswordBox.Password = "";
        }

        private void StartServer()
        {
            server = new TcpListener(IPAddress.Any, 5000);
            server.Start();

            Thread t = new Thread(() =>
            {
                while (true)
                {
                    var client = server.AcceptTcpClient();

                    Dispatcher.Invoke(() =>
                    {
                        clients.Add(client);
                    });
                }
            });

            t.IsBackground = true;
            t.Start();
        }

        private void Send(TcpClient client, string msg)
        {
            try
            {
                var stream = client.GetStream();
                byte[] data = Encoding.UTF8.GetBytes(msg);
                stream.Write(data, 0, data.Length);
            }
            catch
            {
                MessageBox.Show("Błąd wysyłania do klienta");
            }
        }

        private void Block_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsList.SelectedItem is TcpClient c)
                Send(c, "BLOCK");
        }

        private void Unblock_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsList.SelectedItem is TcpClient c)
                Send(c, "UNBLOCK");
        }

        private void StartTimer_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsList.SelectedItem is TcpClient c &&
                int.TryParse(TimerBox.Text, out int sec))
            {
                Send(c, $"TIMER:{sec}");
            }
            else
            {
                MessageBox.Show("Podaj poprawną liczbę sekund i wybierz klienta");
            }
        }
    }
}
