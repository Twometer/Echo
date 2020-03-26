using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Echo.Client.Network;
using Echo.Network.Packets;
using Echo.Network.Util;
using Echo.Client.Util;

namespace Echo.Client.Views
{
    public class RegisterWindow : Window
    {
        private TextBox nicknameBox;
        private TextBox passwordBox;
        private TextBox passwordConfirmBox;
        private Button cancelButton;
        private Button createButton;

        public RegisterWindow()
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            nicknameBox = this.FindControl<TextBox>("NicknameBox");
            passwordBox = this.FindControl<TextBox>("PasswordBox");
            passwordConfirmBox = this.FindControl<TextBox>("PasswordConfirmBox");
            cancelButton = this.FindControl<Button>("CancelButton");
            createButton = this.FindControl<Button>("CreateButton");
            cancelButton.Click += CancelButton_Click;
            createButton.Click += CreateButton_Click;
        }

        private async void CreateButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            P03CreateAccountReply reply = await EchoClient.Instance.SendRequest<P03CreateAccountReply>(new P02CreateAccount() { Nickname = nicknameBox.Text, PasswordHash = Hash.Sha256(passwordBox.Text) });
            if (reply.Status == P03CreateAccountReply.StatusCode.Ok)
            {
                await MsgBox.Show("Account created", "Welcome to Echo! Your tag is: " + reply.EchoTag + ". You can now log in.", MessageBox.Avalonia.Enums.Icon.Info);
                Close(true);
            }
            else if (reply.Status == P03CreateAccountReply.StatusCode.InvalidName)
            {
                await MsgBox.Show("Error", "Your account name is not valid.");
            }
            else if (reply.Status == P03CreateAccountReply.StatusCode.NameTaken)
            {
                await MsgBox.Show("Error", "Well, this should not happen. Apparently over 10k users already chose this username, so please choose a different one.");
            }
        }

        private void CancelButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Close(false);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
