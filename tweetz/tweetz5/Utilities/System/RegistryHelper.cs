using System.Reflection;
using Microsoft.Win32;

// ReSharper disable PossibleNullReferenceException

namespace tweetz5.Utilities.System
{
    public static class RegistryHelper
    {
        private const string ApplicationName = "Tweetz Desktop";

        private static RegistryKey OpenStartupSubKey()
        {
            return Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        }

        public static void RegisterInStartup(bool register)
        {
            using(var registryKey = OpenStartupSubKey())
            {
                if (register)
                {
                    var path = $"\"{Assembly.GetExecutingAssembly().Location}\"";
                    registryKey.SetValue(ApplicationName, path);
                }
                else
                {
                    registryKey.DeleteValue(ApplicationName);
                }
            }
        }

        public static bool IsRegisteredInStartup()
        {
            using (var registryKey = OpenStartupSubKey())
            {
                return registryKey.GetValue(ApplicationName) != null;
            }
        }
    }
}