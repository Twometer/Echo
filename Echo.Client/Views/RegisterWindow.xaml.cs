using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Echo.Client.Views
{
    public class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
