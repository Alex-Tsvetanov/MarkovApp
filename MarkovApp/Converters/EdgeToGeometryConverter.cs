using MarkovApp.Models;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace MarkovApp.Converters
{
    public class EdgeToGeometryConverter : IValueConverter
    {
        private const double NodeRadius = 20;
        private const double CurveOffset = 40;

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not Edge edge)
            {
                return null;
            }

            Point start = new Point(edge.ToNode.X + NodeRadius, edge.ToNode.Y + NodeRadius);
            Point end = new Point(edge.FromNode.X + NodeRadius, edge.FromNode.Y + NodeRadius);

            Vector dir = end - start;
            if (dir.Length == 0)
            {
                return null;
            }

            Vector normal = new Vector(-dir.Y, dir.X);
            normal.Normalize();
            normal *= CurveOffset;

            Point control1 = start + dir * 0.25 + normal;
            Point control2 = start + dir * 0.75 + normal;

            PathFigure figure = new PathFigure();
            figure.StartPoint = start;
            figure.Segments.Add(new BezierSegment(control1, control2, end, true));

            return new PathGeometry(new[] { figure });
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}