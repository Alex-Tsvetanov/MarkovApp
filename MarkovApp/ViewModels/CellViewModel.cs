using MarkovApp.Infrastructure;
using MarkovApp.Models;
using System.Globalization;

namespace MarkovApp.ViewModels
{
    public class CellViewModel : ObservableObject
    {
        private readonly Cell _cell;

        public Cell Cell => _cell;
        public int Row => _cell.Row;
        public int Column => _cell.Column;

        private string _text;
        public string Text
        {
            get => _text;
            set
            {
                if (SetProperty(ref _text, value))
                {
                    string normalized = _text.Replace(',', '.');
                    if (double.TryParse(normalized, System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out double val))
                        _cell.Value = val;
                }
            }
        }

        public string TooltipText => $"Cell ({Row}, {Column})";

        public CellViewModel(Cell cell)
        {
            _cell = cell ?? throw new ArgumentNullException(nameof(cell));
            _text = cell.Value.ToString(CultureInfo.InvariantCulture);

            _cell.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Cell.Value))
                {
                    _text = _cell.Value.ToString(CultureInfo.InvariantCulture);
                    OnPropertyChanged(nameof(Text));
                }
            };
        }

        public bool TryGetValue(out double value)
        {
            string normalized = _text.Replace(',', '.');
            return double.TryParse(normalized, System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }
    }
}