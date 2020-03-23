using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Collections.Generic;

namespace Echo.Client.Views
{
    public class MainWindow : Window
    {
        private ListBox channelList;
        private ListBox messageList;
        private ListBox userList;
        private TextBlock statusLabel;

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
            Initialize();
        }

        private async void Initialize()
        {
            var loginWindow = new LoginWindow();
            var result = await loginWindow.ShowDialog<bool>(this);
            if (!result)
            {
                Close();
                return;
            }
        }
    }
}
