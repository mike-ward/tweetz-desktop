using System;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using tweetz5.Utilities.System;

namespace tweetz5.Utilities.ExceptionHandling
{
    internal class CrashReport
    {
        private readonly Exception _exception;
        private readonly string _version;
        private string _osInfo;
        private readonly string _divider = new string('-', 65);

        public CrashReport(Exception exception)
        {
            _exception = exception;
            _version = BuildInfo.Version;
            OperatingSystemInformation();
        }

        private void OperatingSystemInformation()
        {
            try
            {
                var osInfo = new StringBuilder();
                osInfo.AppendLine("Operation System Information");
                osInfo.AppendLine(_divider);
                osInfo.AppendLine(ProductName());
                osInfo.AppendLine("Service Pack: " + NativeMethods.GetServicePack());
                _osInfo = osInfo.ToString();
            }
            catch (Exception)
            {
                _osInfo = "Operating System Information unavailable";
            }
        }

        private static string ProductName()
        {
            try
            {
                const string subKey = @"SOFTWARE\Wow6432Node\Microsoft\Windows NT\CurrentVersion";
                var key = Registry.LocalMachine;
                var skey = key.OpenSubKey(subKey);
                return skey.GetValue("ProductName").ToString();
            }
            catch (Exception)
            {
                return "Product name unavailabe";
            }
        }

        public void ShowDialog()
        {
            var message = new StringBuilder();
            message.AppendLine("Tweetz Desktop Crash Report");
            message.AppendLine("Date: " + DateTime.UtcNow.ToString("u"));
            message.AppendLine("Version: " + _version);
            message.AppendLine();
            message.AppendLine(_divider);
            message.AppendLine("*** Pressing Ctrl+C will copy the contents of this dialog ***");
            message.AppendLine(_divider);
            message.AppendLine();
            message.AppendLine(_osInfo);
            message.AppendLine();
            message.AppendLine("Exception");
            message.AppendLine(_divider);
            message.AppendLine(_exception.ToString());
            MessageBox.Show(message.ToString());
        }
    }
}