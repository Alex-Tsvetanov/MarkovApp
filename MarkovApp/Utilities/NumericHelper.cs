using System.Text;

namespace MarkovApp.Utilities
{
    public static class NumericHelper
    {
        public static string ToString(double[,] matrix, IEnumerable<int>? displayHeaders = null)
        {
            ArgumentNullException.ThrowIfNull(matrix);

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            var sb = new StringBuilder();

            List<int> headers;
            if (displayHeaders != null)
            {
                headers = displayHeaders.ToList();

                if (headers.Count != cols)
                    throw new ArgumentException("displayHeaders length must match matrix column count.");
            }
            else
            {
                headers = Enumerable.Range(1, cols).ToList();
            }

            sb.Append("  i,j    ");
            foreach (var header in headers)
                sb.Append(header.ToString().PadRight(10));
            sb.AppendLine();

            foreach (var i in Enumerable.Range(0, rows))
            {
                sb.Append((i + 1).ToString().PadRight(7));
                foreach (var j in Enumerable.Range(0, cols))
                    sb.Append(matrix[i, j].ToString("F2").PadRight(10));
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public static string VectorToString(IEnumerable<double> vector, int precision = 2)
        {
            ArgumentNullException.ThrowIfNull(vector);
            return string.Join(", ", vector.Select(v => v.ToString($"F{precision}")));
        }

        public static bool TryParseDoubleInvariant(string text, out double value)
        {
            value = 0;

            if (string.IsNullOrWhiteSpace(text))
                return false;

            string normalized = text.Trim().Replace(',', '.');

            bool result = double.TryParse(
                normalized,
                System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture,
                out value
            );

            return result;
        }
    }
}