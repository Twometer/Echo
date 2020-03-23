using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Echo.Client.Views
{
    public class LoginWindow : Window
    {
        private Button registerButton;
        private Button cancelButton;
        private Button loginButton;

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

        private void LoginButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Close(true);
        }
    }
}
