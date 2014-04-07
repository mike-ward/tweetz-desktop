using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using tweetz5.Commands;

#pragma warning disable 169
#pragma warning disable 649
// ReSharper disable InconsistentNaming

namespace tweetz5.Utilities.System
{
    internal static class BuildInfo
    {
        public static bool IsWindows8OrNewer { get; private set; }

        static BuildInfo()
        {
            var os = Environment.OSVersion;
            IsWindows8OrNewer = os.Platform == PlatformID.Win32NT && (os.Version.Major > 6 || (os.Version.Major == 6 && os.Version.Minor >= 2));
        }

        // http://msdn.microsoft.com/en-us/library/ms680313

        private struct _IMAGE_FILE_HEADER
        {
            public ushort Machine;
            public ushort NumberOfSections;
            public uint TimeDateStamp;
            public uint PointerToSymbolTable;
            public uint NumberOfSymbols;
            public ushort SizeOfOptionalHeader;
            public ushort Characteristics;
        };

        private static DateTime GetBuildDateTime()
        {
            var assembly = Assembly.GetCallingAssembly();

            if (File.Exists(assembly.Location))
            {
                var buffer = new byte[Math.Max(Marshal.SizeOf(typeof (_IMAGE_FILE_HEADER)), 4)];
                using (var fileStream = new FileStream(assembly.Location, FileMode.Open, FileAccess.Read))
                {
                    fileStream.Position = 0x3C;
                    fileStream.Read(buffer, 0, 4);
                    fileStream.Position = BitConverter.ToUInt32(buffer, 0); // COFF header offset
                    fileStream.Read(buffer, 0, 4); // "PE\0\0"
                    fileStream.Read(buffer, 0, buffer.Length);
                }
                var pinnedBuffer = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                try
                {
                    var coffHeader = (_IMAGE_FILE_HEADER) Marshal.PtrToStructure(pinnedBuffer.AddrOfPinnedObject(), typeof (_IMAGE_FILE_HEADER));
                    return TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1) + new TimeSpan(coffHeader.TimeDateStamp*TimeSpan.TicksPerSecond));
                }
                finally
                {
                    pinnedBuffer.Free();
                }
            }
            return new DateTime();
        }

        public static bool HasExpired()
        {
            var now = DateTime.Now;
            var buildDate = GetBuildDateTime();
            if (now > buildDate.AddMonths(3))
            {
                var mainWindow = (MainWindow) Application.Current.MainWindow;
                var message = string.Format("Expired\n t: {0:g}\n b: {1:g}\n v: {2}", now, buildDate, Version);
                AlertCommand.Command.Execute(message, mainWindow);
                return true;
            }
            return false;
        }

        public static string Version
        {
            get
            {
                var version = Assembly.GetExecutingAssembly().GetName().Version;
                return string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);
            }
        }
    }
}