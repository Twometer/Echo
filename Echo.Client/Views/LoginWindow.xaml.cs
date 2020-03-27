using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Echo.Client.Network;
using Echo.Client.Util;
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
            if (await EchoClient.Instance.Login(echoTagBox.Text, passwordBox.Text))
                Close(true);
            else
                await MsgBox.Show("Error", "Invalid credentials.");
        }
    }
}
