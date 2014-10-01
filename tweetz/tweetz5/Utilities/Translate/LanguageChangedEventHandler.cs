using System;
using System.Windows;

namespace tweetz5.Utilities.Translate
{
    public class LanguageChangedEventManager : WeakEventManager
    {
        public static void AddListener(TranslationService source, IWeakEventListener listener)
        {
            CurrentManager.ProtectedAddListener(source, listener);
        }

        public static void RemoveListener(TranslationService source, IWeakEventListener listener)
        {
            CurrentManager.ProtectedRemoveListener(source, listener);
        }

        private void OnLanguageChanged(object sender, EventArgs e)
        {
            DeliverEvent(sender, e);
        }

        protected override void StartListening(object source)
        {
            var manager = (TranslationService) source;
            manager.LanguageChanged += OnLanguageChanged;
        }

        protected override void StopListening(Object source)
        {
            var manager = (TranslationService) source;
            manager.LanguageChanged -= OnLanguageChanged;
        }

        private static LanguageChangedEventManager CurrentManager
        {
            get
            {
                var managerType = typeof (LanguageChangedEventManager);
                var manager = (LanguageChangedEventManager) GetCurrentManager(managerType);
                if (manager == null)
                {
                    manager = new LanguageChangedEventManager();
                    SetCurrentManager(managerType, manager);
                }
                return manager;
            }
        }
    }
}