using MarkovApp.Infrastructure;

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
            set => SetProperty(ref _value, value);
        }
        public Cell(int row, int column, double value)
        {
            Row = row;
            Column = column;
            _value = value;
        }
    }
}