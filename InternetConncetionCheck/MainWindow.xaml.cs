using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace InternetConncetionCheck
{
    public partial class MainWindow : Window
    {
        private CancellationTokenSource cancellationTokenSource;

        public MainWindow()
        {
            InitializeComponent();
            InitializeBackgroundPing();

        }

        /// <summary>
        /// Checks whether the internet connection is good, slow, or disconnected.
        /// </summary>
        /// <returns>A string indicating the connection status: "Connected", "Poor Connection", or "No Internet".</returns>
        private static async Task<string> CheckConnectionStatusAsync()
        {
            try
            {
                Ping ping = new Ping();
                PingReply reply = await ping.SendPingAsync("8.8.8.8", 3000); // 3-second timeout

                if (reply.Status == IPStatus.Success)
                {
                    // Check round-trip time (RTT) to determine if the connection is slow
                    if (reply.RoundtripTime > 200)
                    {
                        return "Poor Connection";
                    }
                    else
                    {
                        return "Connected";
                    }
                }
                else
                {
                    return "No Internet";
                }
            }
            catch
            {
                return "No Internet";
            }
        }

        /// <summary>
        /// Initializes the background ping using a CancellationToken.
        /// </summary>
        private void InitializeBackgroundPing()
        {
            cancellationTokenSource = new CancellationTokenSource();
            Task.Run(() => BackgroundPingAsync(cancellationTokenSource.Token));
        }

        private async Task BackgroundPingAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                string connectionStatus = await CheckConnectionStatusAsync();
                Dispatcher.Invoke(() =>
                {
                    textInternet.Text = connectionStatus;

                    switch (connectionStatus)
                    {
                        case "Connected":
                            btnName.Background = new SolidColorBrush(Colors.Green);
                            break;
                        case "No Internet":
                            btnName.Background = new SolidColorBrush(Colors.Red);
                            break;
                        case "Poor Connection":
                            btnName.Background = new SolidColorBrush(Colors.Yellow);
                            break;
                    }
                });
                await Task.Delay(1000);
            }
        }

        /// <summary>
        /// Call this when you want to stop the background ping, e.g., when the window is closing.
        /// </summary>
        private void StopBackgroundPing()
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            StopBackgroundPing();
            base.OnClosed(e);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            InitializeBackgroundPing();
            //textInternet.Text = "second";
        }
    }
}
