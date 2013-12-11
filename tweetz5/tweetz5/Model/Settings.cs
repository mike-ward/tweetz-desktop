// Copyright (c) 2013 Blue Onion Software - All rights reserved

using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using tweetz5.Annotations;

namespace tweetz5.Model
{
    public class Settings : INotifyPropertyChanged
    {
        public static Settings ApplicationSettings = new Settings();

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

        public double Width
        {
            get { return Properties.Settings.Default.Width; }
            set
            {
                if (Math.Abs(Properties.Settings.Default.Width - value) > .0001)
                {
                    Properties.Settings.Default.Width = value;
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
            get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }

        public bool IsRegisteredInStartup
        {
            get { return Utilities.System.RegistryHelper.IsRegisteredInStartup(); }
            set
            {
                if (IsRegisteredInStartup != value)
                {
                    Utilities.System.RegistryHelper.RegisterInStartup(value);
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}