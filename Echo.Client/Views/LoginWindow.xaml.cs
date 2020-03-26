using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Echo.Client.Network;
using Echo.Network.Packets;
using Echo.Network.Util;
using MessageBox.Avalonia;

namespace Echo.Client.Views
{
    public class LoginWindow : Window
    {
        private Button registerButton;
        private Button cancelButton;
        private Button loginButton;

        private TextBox echoTagBox;
        private TextBox passwordBox;

        public LoginWindow()
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            registerButton = this.FindControl<Button>("RegisterButton");
            cancelButton = this.FindControl<Button>("CancelButton");
            loginButton = this.FindControl<Button>("LoginButton");
            echoTagBox = this.FindControl<TextBox>("EchoTagBox");
            passwordBox = this.FindControl<TextBox>("PasswordBox");
            registerButton.Click += RegisterButton_Click;
            cancelButton.Click += CancelButton_Click;
            loginButton.Click += LoginButton_Click;
        }

        private void CancelButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Close(false);
        }

        private void RegisterButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            new RegisterWindow().ShowDialog(this);
        }

        private async void LoginButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            P05CreateSessionReply reply = await EchoClient.Instance.SendRequest<P05CreateSessionReply>(new P04CreateSession() { EchoTag = echoTagBox.Text, KeyHash = Hash.Sha256(passwordBox.Text) });
            if (reply.Authenticated)
                Close(true);
            else
            {
                await MessageBoxManager.GetMessageBoxStandardWindow("Error", "Invalid credentials").Show();
            }
        }
    }
}
