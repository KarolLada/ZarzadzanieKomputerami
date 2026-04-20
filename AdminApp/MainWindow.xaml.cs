using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace AdminApp
{
    public partial class MainWindow : Window
    {
        TcpListener server;
        ObservableCollection<TcpClient> clients = new ObservableCollection<TcpClient>();
        bool logged = false;

        public MainWindow()
        {
            InitializeComponent();
            ClientsList.ItemsSource = clients;
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (PasswordBox.Password == "admin")
            {
                logged = true;
                StartServer();
                MessageBox.Show("Zalogowano!");
            }
            else
            {
                MessageBox.Show("Złe hasło");
            }
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

        private void SendToClient(TcpClient client, string msg)
        {
            var stream = client.GetStream();
            byte[] data = Encoding.UTF8.GetBytes(msg);
            stream.Write(data, 0, data.Length);
        }

        private void Block_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsList.SelectedItem is TcpClient client)
            {
                SendToClient(client, "BLOCK");
            }
        }

        private void Unblock_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsList.SelectedItem is TcpClient client)
            {
                SendToClient(client, "UNBLOCK");
            }
        }
    }
}