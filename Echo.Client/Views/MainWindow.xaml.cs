using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Echo.Client.Network;
using Echo.Client.Util;
using Echo.Client.Voice;
using Echo.Network.Model;
using Echo.Network.Packets;
using Echo.Network.Packets.Tcp;
using MessageBox.Avalonia;
using System.Collections.Generic;

namespace Echo.Client.Views
{
    public class MainWindow : Window
    {
        private ListBox channelList;
        private ListBox messageList;
        private ListBox userList;
        private TextBlock statusLabel;
        private TextBlock serverLabel;

        private VoiceManager voiceManager = new VoiceManager();

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            channelList = this.FindControl<ListBox>("ChannelList");
            messageList = this.FindControl<ListBox>("MessageList");
            userList = this.FindControl<ListBox>("UserList");
            statusLabel = this.FindControl<TextBlock>("StatusLabel");
            serverLabel = this.FindControl<TextBlock>("ServerLabel");
            Initialize();

            channelList.DoubleTapped += ChannelList_DoubleTapped;
            this.Closing += MainWindow_Closing;

            EchoClient.Instance.ServerInfoChanged += Client_ServerInfoChanged;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            voiceManager.EndStreaming();
        }

        private async void ChannelList_DoubleTapped(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (channelList.SelectedItem is Channel channel)
            {
                statusLabel.Text = "Joining channel...";
                P11JoinChannelReply reply = await EchoClient.Instance.SendRequest<P11JoinChannelReply>(new P10JoinChannel() { ChannelId = channel.ChannelId });
                if (reply.Status == P11JoinChannelReply.StatusCode.Unauthorized)
                {
                    statusLabel.Text = "Insufficient permissions";
                }
                else if (reply.Status == P11JoinChannelReply.StatusCode.InvalidChannel)
                {
                    statusLabel.Text = "Invalid channel";
                }
                else if (reply.Status == P11JoinChannelReply.StatusCode.Ok)
                {
                    statusLabel.Text = "Connecting voice... [" + reply.VoiceUrl + "; " + reply.VoiceToken + "]";
                    EchoClient.Instance.VoiceClient = new VoiceClient(reply.VoiceUrl, reply.VoiceToken);
                    var ok = await EchoClient.Instance.VoiceClient.Connect();
                    if (ok)
                    {
                        voiceManager.BeginStreaming();
                        statusLabel.Text = "Voice connection established";
                    }
                    else
                    {
                        statusLabel.Text = "Failed to connect voice";
                    }
                }
            }
        }

        private void Client_ServerInfoChanged(object sender, System.EventArgs e)
        {
            serverLabel.Text = EchoClient.Instance.ServerInfo.ServerName;
            channelList.Items = EchoClient.Instance.ServerInfo.Channels;
            userList.Items = EchoClient.Instance.ServerInfo.Users;
        }

        private async void Initialize()
        {
            statusLabel.Text = "Connecting...";
            try
            {
                await EchoClient.Instance.Connect(Constants.Host);
            }
            catch
            {
                statusLabel.Text = "Server not reachable";
                await MsgBox.Show("Error", "Could not connect to server. Please try again later.");
                Close();
                return;
            }

            statusLabel.Text = "Connected.";

            var loginWindow = new LoginWindow();
            var result = await loginWindow.ShowDialog<bool>(this);
            if (!result)
            {
                Close();
                return;
            }

            statusLabel.Text = "Logged in as " + EchoClient.Instance.UserInfo.Tag;
        }
    }
}
