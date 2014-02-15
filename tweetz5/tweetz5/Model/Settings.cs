using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using tweetz5.Utilities.System;

namespace tweetz5.Model
{
    public sealed class Settings : INotifyPropertyChanged
    {
        public static readonly Settings ApplicationSettings = new Settings();

        public bool Chirp
        {
            get { return Properties.Settings.Default.Chirp; }
            set
            {
                if (Properties.Settings.Default.Chirp != value)
                {
                    Properties.Settings.Default.Chirp = value;
                    Properties.Settings.Default.Save();
                    OnPropertyChanged();
                }
            }
        }

        public bool ShowMedia
        {
            get { return Properties.Settings.Default.ShowMedia; }
            set
            {
                if (Properties.Settings.Default.ShowMedia != value)
                {
                    Properties.Settings.Default.ShowMedia = value;
                    Properties.Settings.Default.Save();
                    OnPropertyChanged();
                }
            }
        }

        public string FontSize
        {
            get { return Properties.Settings.Default.FontSize; }
            set
            {
                if (Properties.Settings.Default.FontSize != value)
                {
                    Properties.Settings.Default.FontSize = value;
                    Properties.Settings.Default.Save();
                    OnPropertyChanged();
                }
            }
        }

        public bool SpellCheck
        {
            get { return Properties.Settings.Default.SpellCheck; }
            set
            {
                if (Properties.Settings.Default.SpellCheck != value)
                {
                    Properties.Settings.Default.SpellCheck = value;
                    Properties.Settings.Default.Save();
                    OnPropertyChanged();
                }
            }
        }

        public static string Version
        {
            get
            {
                var version = Assembly.GetExecutingAssembly().GetName().Version;
                return string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);
            }
        }

        public bool IsRegisteredInStartup
        {
            get { return RegistryHelper.IsRegisteredInStartup(); }
            set
            {
                if (IsRegisteredInStartup != value)
                {
                    RegistryHelper.RegisterInStartup(value);
                    OnPropertyChanged();
                }
            }
        }

        public string Theme
        {
            get { return Properties.Settings.Default.Theme; }
            set
            {
                if (Properties.Settings.Default.Theme != value)
                {
                    Properties.Settings.Default.Theme = value;
                    Properties.Settings.Default.Save();
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}