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
        private string _osInfo;
        private const string _divider = "---------------------------------------------------------";

        public CrashReport(Exception exception)
        {
            _exception = exception;
            ProductName();
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
            message.AppendLine("Tweetz Desktop Crash Report - " + BuildInfo.Version);
            message.AppendLine(_divider);
            message.AppendLine();
            message.AppendLine("*** Pressing Ctrl+C will copy the contents of this dialog ***");
            message.AppendLine();
            message.AppendLine(_osInfo);
            message.AppendLine();
            message.AppendLine("Stack Trace");
            message.AppendLine(_divider);
            message.AppendLine(_exception.ToString());

            MessageBox.Show(message.ToString());
            Environment.Exit(110);
        }
    }
}