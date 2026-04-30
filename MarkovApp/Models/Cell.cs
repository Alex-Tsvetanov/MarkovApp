using MarkovApp.Infrastructure;
using System.Globalization;

namespace MarkovApp.Models
{
    public class Cell : ObservableObject
    {
        public int Row { get; }
        public int Column { get; }

        private double _value;
        public double Value
        {
            get => _value;
            set
            {
                if (SetProperty(ref _value, value))
                {
                    string normalized = _text?.Replace(',', '.') ?? "";
                    bool textAlreadyMatches = double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out double parsed) && parsed == value;
                    if (!textAlreadyMatches)
                    {
                        _text = value.ToString(CultureInfo.InvariantCulture);
                        OnPropertyChanged(nameof(Text));
                    }
                }
            }
        }

        private string _text;
        public string Text
        {
            get => _text;
            set
            {
                if (SetProperty(ref _text, value))
                {
                    string normalized = value.Replace(',', '.');
                    if (double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out double val) && val != _value)
                    {
                        _value = val;
                        OnPropertyChanged(nameof(Value));
                    }
                }
            }
        }

        public Cell(int row, int column, double value)
        {
            Row = row;
            Column = column;
            _value = value;
            _text = value.ToString(CultureInfo.InvariantCulture);
        }
    }
}