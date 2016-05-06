using System;
using System.ComponentModel;
using System.Windows;

namespace tweetz5.Utilities.Translate
{
    public class Translater : IWeakEventListener, INotifyPropertyChanged
    {
        private readonly string _key;
        public event PropertyChangedEventHandler PropertyChanged;

        public Translater(string key)
        {
            _key = key;
            LanguageChangedEventManager.AddListener(TranslationService.Instance, this);
        }

        public object Value
        {
            get { return TranslationService.Instance.Translate(_key); }
        }

        private void OnLanguageChanged(object sender, EventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Value"));
        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (managerType == typeof(LanguageChangedEventManager))
            {
                OnLanguageChanged(sender, e);
                return true;
            }
            return false;
        }
    }
}