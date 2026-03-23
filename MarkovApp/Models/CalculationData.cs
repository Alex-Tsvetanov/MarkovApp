using System.Text.Json.Serialization;

namespace MarkovApp.Models
{
    public class CalculationData
    {
        private const double Tolerance = 1e-6;

        [JsonIgnore]
        public double[,] TransitionMatrix { get; set; }

        public double[] InitialStateVector { get; set; }

        public double[][] TransitionMatrixSerializable
        {
            get
            {
                int rows = TransitionMatrix.GetLength(0);
                int cols = TransitionMatrix.GetLength(1);

                return Enumerable.Range(0, rows)
                    .Select(i => Enumerable.Range(0, cols)
                        .Select(j => TransitionMatrix[i, j])
                        .ToArray())
                    .ToArray();
            }
            set
            {
                if (value == null || value.Length == 0)
                    throw new ArgumentException("Transition matrix cannot be null or empty.");

                int cols = value[0].Length;
                if (cols == 0 || value.Any(row => row.Length != cols))
                    throw new ArgumentException("All rows of the transition matrix must have the same length and not be empty.");

                int rows = value.Length;
                TransitionMatrix = new double[rows, cols];

                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < cols; j++)
                        TransitionMatrix[i, j] = value[i][j];
            }
        }

        public CalculationData(double[,] transitionMatrix, double[] initialStateVector)
        {
            TransitionMatrix = transitionMatrix ?? throw new ArgumentNullException(nameof(transitionMatrix));
            InitialStateVector = initialStateVector ?? throw new ArgumentNullException(nameof(initialStateVector));
        }

        public CalculationData()
        {
            TransitionMatrix = new double[0, 0];
            InitialStateVector = [];
        }
    }
}