using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Resources;

namespace WPFUtilsLib.Services.TranslationService
{
    public class TranslationService : INotifyPropertyChanged
    {
        private static readonly TranslationService _translationService = new();
        public static TranslationService Instance
        {
            get { return _translationService; }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly Dictionary<string, ResourceManager> _resourceManagers = new();
        private CultureInfo _culture = CultureInfo.CurrentCulture;

        public string? this[string key]
        {
            get
            {
                var (baseName, stringName) = SplitName(key);
                string? translation = null;

                if (_resourceManagers.ContainsKey(baseName))
                {
                    translation = _resourceManagers[baseName].GetString(stringName, _culture);
                }

                return translation ?? key;
            }
        }

        public CultureInfo CurrentCulture
        {
            get { return _culture; }
            set
            {
                if (_culture != value)
                {
                    _culture = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
                }
            }
        }

        public void AddResourceManager(ResourceManager resourceManager)
        {
            if (!_resourceManagers.ContainsKey(resourceManager.BaseName))
            {
                _resourceManagers.Add(resourceManager.BaseName, resourceManager);
            }
        }

        public static (string baseName, string stringName) SplitName(string name)
        {
            int index = name.LastIndexOf('.');
            return (name.Substring(0, index), name.Substring(index + 1));
        }
    }
}
