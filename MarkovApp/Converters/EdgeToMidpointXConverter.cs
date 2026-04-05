using MarkovApp.Models;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MarkovApp.Converters
{
    public class EdgeToMidpointXConverter : IValueConverter
    {
        private const double NodeRadius = 20;
        private const double CurveOffset = 40;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not Edge edge)
                return 0.0;

            Point start = new Point(edge.ToNode.X + NodeRadius, edge.ToNode.Y + NodeRadius);
            Point end = new Point(edge.FromNode.X + NodeRadius, edge.FromNode.Y + NodeRadius);

            Vector dir = end - start;
            if (dir.Length == 0)
                return 0.0;

            Vector normal = new Vector(-dir.Y, dir.X);
            normal.Normalize();
            normal *= CurveOffset;

            Point control1 = start + dir * 0.25 + normal;
            Point control2 = start + dir * 0.75 + normal;

            return 0.125 * start.X + 0.375 * control1.X + 0.375 * control2.X + 0.125 * end.X;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
