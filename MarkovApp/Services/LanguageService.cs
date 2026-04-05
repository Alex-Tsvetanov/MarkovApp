using System.Windows;

namespace MarkovApp.Services
{
    public class LanguageService
    {
        private bool _isEnglish = true;

        public bool IsEnglish => _isEnglish;

        public void Toggle()
        {
            _isEnglish = !_isEnglish;
            Apply();
        }

        public void Apply()
        {
            var dict = new ResourceDictionary
            {
                Source = new Uri(
                    _isEnglish
                        ? "pack://application:,,,/Localization/Strings.en.xaml"
                        : "pack://application:,,,/Localization/Strings.bg.xaml")
            };

            var appDicts = Application.Current.Resources.MergedDictionaries;
            var existing = appDicts.FirstOrDefault(d =>
                d.Source != null &&
                (d.Source.OriginalString.Contains("Strings.en") ||
                 d.Source.OriginalString.Contains("Strings.bg")));

            if (existing != null)
                appDicts.Remove(existing);

            appDicts.Add(dict);
        }
    }
}
