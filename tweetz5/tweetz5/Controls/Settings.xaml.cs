// Copyright (c) 2013 Blue Onion Software - All rights reserved

using System.ComponentModel;
using System.Runtime.CompilerServices;
using tweetz5.Annotations;

namespace tweetz5.Controls
{
    public partial class Settings : INotifyPropertyChanged
    {
        public Settings()
        {
            InitializeComponent();
        }

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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}