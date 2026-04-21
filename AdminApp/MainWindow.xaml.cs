using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Threading.Tasks;
 
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
 
        // TOAST
        private async void ShowToast(string message, string color = "#4CAF50")
        {
            ToastText.Text = message;
 
            Toast.Background = (Brush)new BrushConverter().ConvertFromString(color);
 
            Toast.Visibility = Visibility.Visible;
 
            for (double i = 0; i <= 1; i += 0.1)
            {
                Toast.Opacity = i;
                await Task.Delay(15);
            }
 
            await Task.Delay(2000);
 
            for (double i = 1; i >= 0; i -= 0.1)
            {
                Toast.Opacity = i;
                await Task.Delay(15);
            }
 
            Toast.Visibility = Visibility.Collapsed;
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
 
                ShowToast("Zalogowano pomyślnie ✅", "#4CAF50");
            }
            else
            {
                ShowToast("Złe hasło ❌", "#F44336");
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
                ShowToast("Błąd wysyłania do klienta", "#F44336");
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
                ShowToast("Wybierz klienta i podaj liczbę sekund", "#F44336");
            }
        }
    }
}
