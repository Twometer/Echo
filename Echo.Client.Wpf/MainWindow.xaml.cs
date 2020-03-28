using Echo.Client.Network;
using Echo.Client.Wpf.Voice;
using Echo.Network.Model;
using Echo.Network.Packets.Tcp;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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

namespace Echo.Client.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private VoiceManager voiceManager = new VoiceManager();

        public MainWindow()
        {
            InitializeComponent();

            if (!Debugger.IsAttached)
                ServerBox.Text = "echo.twometer.de";

            EchoClient.Instance.ServerInfoChanged += EchoClient_ServerInfoChanged;
        }

        private void EchoClient_ServerInfoChanged(object sender, EventArgs e)
        {
            ServerNameLabel.Text = EchoClient.Instance.ServerInfo.ServerName;

            ChannelList.Items.Clear();
            foreach (var ch in EchoClient.Instance.ServerInfo.Channels)
                ChannelList.Items.Add(ch);

            UserList.Items.Clear();
            foreach (var usr in EchoClient.Instance.ServerInfo.Users)
                UserList.Items.Add(usr);
        }

        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            ConnectButton.IsEnabled = false;
            RegisterButton.IsEnabled = false;
            ConnectButton.Content = "Connecting...";
            await EchoClient.Instance.Connect(ServerBox.Text);
            StatusLabel.Text = $"Logging in...";

            if (await EchoClient.Instance.Login(EchoTagBox.Text, PasswordBox.Password))
            {
                StatusLabel.Text = $"Logged in as {EchoClient.Instance.UserInfo.Tag}";
                DialogHost.CurrentSession.Close();
            }
            else
            {
                StatusLabel.Text = $"Connected.";
                MessageBox.Show("Invalid credentials", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ConnectButton.IsEnabled = true;
                RegisterButton.IsEnabled = true;
                ConnectButton.Content = "Connect";
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void ChannelList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ChannelList.SelectedItem is Channel channel)
            {
                StatusLabel.Text = "Joining channel...";
                P11JoinChannelReply reply = await EchoClient.Instance.SendRequest<P11JoinChannelReply>(new P10JoinChannel() { ChannelId = channel.ChannelId });
                if (reply.Status == P11JoinChannelReply.StatusCode.Unauthorized)
                {
                    StatusLabel.Text = "Insufficient permissions";
                }
                else if (reply.Status == P11JoinChannelReply.StatusCode.InvalidChannel)
                {
                    StatusLabel.Text = "Invalid channel";
                }
                else if (reply.Status == P11JoinChannelReply.StatusCode.Ok)
                {
                    StatusLabel.Text = "Connecting voice... [" + reply.VoiceUrl + "; " + reply.VoiceToken + "]";
                    EchoClient.Instance.VoiceClient = new VoiceClient(reply.VoiceUrl, reply.VoiceToken);
                    var ok = await EchoClient.Instance.VoiceClient.Connect();
                    if (ok)
                    {
                        voiceManager.BeginStreaming();
                        StatusLabel.Text = "Voice connection established";
                    }
                    else
                    {
                        StatusLabel.Text = "Failed to connect voice";
                    }
                }
            }
        }
    }
}
