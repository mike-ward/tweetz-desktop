using System.ComponentModel;
using System.Runtime.CompilerServices;
using tweetz5.Utilities.System;

namespace tweetz5.Model
{
    public sealed class Settings : INotifyPropertyChanged
    {
        public static readonly Settings ApplicationSettings = new Settings();

        public bool Chirp
        {
            get => Properties.Settings.Default.Chirp;
            set
            {
                if (Properties.Settings.Default.Chirp == value) return;
                Properties.Settings.Default.Chirp = value;
                Properties.Settings.Default.Save();
                OnPropertyChanged();
            }
        }

        public bool ShowMedia
        {
            get => Properties.Settings.Default.ShowMedia;
            set
            {
                if (Properties.Settings.Default.ShowMedia == value) return;
                Properties.Settings.Default.ShowMedia = value;
                Properties.Settings.Default.Save();
                OnPropertyChanged();
            }
        }

        public string FontSize
        {
            get => Properties.Settings.Default.FontSize;
            set
            {
                if (Properties.Settings.Default.FontSize == value) return;
                Properties.Settings.Default.FontSize = value;
                Properties.Settings.Default.Save();
                OnPropertyChanged();
            }
        }

        public bool SpellCheck
        {
            get => Properties.Settings.Default.SpellCheck;
            set
            {
                if (Properties.Settings.Default.SpellCheck == value) return;
                Properties.Settings.Default.SpellCheck = value;
                Properties.Settings.Default.Save();
                OnPropertyChanged();
            }
        }

        public bool ShowInTaskbar
        {
            get => Properties.Settings.Default.ShowInTaskbar;
            set
            {
                if (Properties.Settings.Default.ShowInTaskbar == value) return;
                Properties.Settings.Default.ShowInTaskbar = value;
                Properties.Settings.Default.Save();
                OnPropertyChanged();
            }
        }

        public static string Version => BuildInfo.Version;

        public bool IsRegisteredInStartup
        {
            get => RegistryHelper.IsRegisteredInStartup();
            set
            {
                if (IsRegisteredInStartup == value) return;
                RegistryHelper.RegisterInStartup(value);
                OnPropertyChanged();
            }
        }

        public string Theme
        {
            get => Properties.Settings.Default.Theme;
            set
            {
                if (Properties.Settings.Default.Theme == value) return;
                Properties.Settings.Default.Theme = value;
                Properties.Settings.Default.Save();
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}