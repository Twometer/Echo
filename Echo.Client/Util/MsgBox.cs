using MB = MessageBox.Avalonia;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Echo.Client.Util
{
    public class MsgBox
    {

        public static async Task Show(string title, string message, MB.Enums.Icon icon = MB.Enums.Icon.Error)
        {
            await MB.MessageBoxManager.GetMessageBoxStandardWindow(title, message, MB.Enums.ButtonEnum.Ok, icon).Show();
        }

    }
}
