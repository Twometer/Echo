using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Echo.Client.Network;
using Echo.Client.Util;
using MessageBox.Avalonia;
using System.Collections.Generic;

namespace Echo.Client.Views
{
    public class MainWindow : Window
    {
        private const string Host = "localhost";

        private ListBox channelList;
        private ListBox messageList;
        private ListBox userList;
        private TextBlock statusLabel;
        private TextBlock serverLabel;

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

            EchoClient.Instance.ServerInfoChanged += Client_ServerInfoChanged;
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
                await EchoClient.Instance.Connect(Host);
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
