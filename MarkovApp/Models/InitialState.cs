using MarkovApp.Infrastructure;
using System.Globalization;

namespace MarkovApp.Models
{
    public class InitialState : ObservableObject
    {
        public int Index { get; }

        private string _text;
        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }
        public InitialState(int index, double value)
        {
            Index = index;
            _text = value.ToString(CultureInfo.InvariantCulture);
        }

        public bool TryGetValue(out double value)
        {
            string normalized = _text.Replace(',', '.');
            return double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }
    }
}