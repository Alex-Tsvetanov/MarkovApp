using MarkovApp.Configuration;
using MarkovApp.Models;
using MarkovApp.Services.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Microsoft.Extensions.Options;

namespace MarkovApp.Services
{
    public class MarkovCalculatorService : IMarkovCalculatorService
    {
        private readonly CalculationSettings _settings;

        public MarkovCalculatorService(IOptions<CalculationSettings> settings)
        {
            _settings = settings.Value;
        }

        public RegularMatrixResult CalculateRegularChain(CalculationData data, int? maxIterations = null)
        {
            ArgumentNullException.ThrowIfNull(data);
            if (data.TransitionMatrix == null || data.InitialStateVector == null)
                throw new ArgumentException("TransitionMatrix and InitialStateVector cannot be null.");
            int n = data.TransitionMatrix.GetLength(0);
            if (data.TransitionMatrix.GetLength(1) != n)
                throw new ArgumentException("TransitionMatrix must be square.");
            if (data.InitialStateVector.Length != n)
                throw new ArgumentException("InitialStateVector length must match TransitionMatrix size.");

            maxIterations ??= _settings.DefaultMaxIterations;

            var matrix = Matrix<double>.Build.DenseOfArray(data.TransitionMatrix);
            var stateVector = Vector<double>.Build.Dense(data.InitialStateVector);

            var current = matrix.Clone();
            var next = matrix.Clone();
            bool isRegular = false;

            for (int iter = 0; iter < maxIterations; iter++)
            {
                next = matrix * current;

                double norm = 0;
                int rows = next.RowCount;
                int cols = next.ColumnCount;

                foreach (var i in Enumerable.Range(0, rows))
                    foreach (var j in Enumerable.Range(0, cols))
                        norm += System.Math.Abs(next[i, j] - current[i, j]);

                if (norm < _settings.ConvergenceEpsilon)
                {
                    isRegular = true;
                    break;
                }

                current = next.Clone();
            }

            Vector<double> limitingProbabilities = isRegular ? next.Row(0) : Vector<double>.Build.Dense(0);

            var resultVector = stateVector * next;

            return new RegularMatrixResult(
                RoundMatrix(next),
                RoundVector(limitingProbabilities),
                RoundVector(resultVector)
            );
        }

        public AbsorbingMatrixResult CalculateAbsorbingChain(CalculationData data)
        {
            ArgumentNullException.ThrowIfNull(data);
            if (data.TransitionMatrix == null || data.InitialStateVector == null)
                throw new ArgumentException("TransitionMatrix and InitialStateVector cannot be null.");
            int n = data.TransitionMatrix.GetLength(0);
            if (data.TransitionMatrix.GetLength(1) != n)
                throw new ArgumentException("TransitionMatrix must be square.");
            if (data.InitialStateVector.Length != n)
                throw new ArgumentException("InitialStateVector length must match TransitionMatrix size.");
            var matrix = Matrix<double>.Build.DenseOfArray(data.TransitionMatrix);

            var absorbingStates = Enumerable.Range(0, n)
                .Where(i => Enumerable.Range(0, n)
                    .All(j => i == j ? System.Math.Abs(matrix[i, j] - 1) <= 1e-9 : System.Math.Abs(matrix[i, j]) <= 1e-9))
                .ToList();

            int nonAbsorbingCount = n - absorbingStates.Count;

            if (nonAbsorbingCount == 0)
                return new AbsorbingMatrixResult(new double[0, 0], new double[0, 0], absorbingStates);

            var Q = Matrix<double>.Build.Dense(nonAbsorbingCount, nonAbsorbingCount);
            var R = Matrix<double>.Build.Dense(nonAbsorbingCount, absorbingStates.Count);

            int row = 0;
            foreach (var i in Enumerable.Range(0, n))
            {
                if (absorbingStates.Contains(i))
                    continue;

                int qCol = 0;
                foreach (var j in Enumerable.Range(0, n))
                {
                    if (absorbingStates.Contains(j))
                    {
                        int index = absorbingStates.IndexOf(j);
                        R[row, index] = matrix[i, j];
                    }
                    else
                    {
                        Q[row, qCol] = matrix[i, j];
                        qCol++;
                    }
                }

                row++;
            }

            var identity = Matrix<double>.Build.DenseIdentity(nonAbsorbingCount);
            var T = (identity - Q).Solve(Matrix<double>.Build.DenseIdentity(nonAbsorbingCount));
            var probabilities = T * R;

            return new AbsorbingMatrixResult(
                RoundMatrix(T),
                RoundMatrix(probabilities),
                absorbingStates
            );
        }

        public bool IsAbsorbingMatrix(double[,] arr)
        {
            var matrix = Matrix<double>.Build.DenseOfArray(arr);
            int n = matrix.RowCount;

            return Enumerable.Range(0, n)
                .Any(i => Enumerable.Range(0, n)
                    .All(j => i == j ? System.Math.Abs(matrix[i, j] - 1) <= 1e-9 : System.Math.Abs(matrix[i, j]) <= 1e-9));
        }

        private static double[,] RoundMatrix(Matrix<double> matrix)
        {
            int rows = matrix.RowCount;
            int cols = matrix.ColumnCount;
            double[,] result = new double[rows, cols];

            foreach (var i in Enumerable.Range(0, rows))
                foreach (var j in Enumerable.Range(0, cols))
                    result[i, j] = System.Math.Round(matrix[i, j], 3);

            return result;
        }

        private static double[] RoundVector(Vector<double> vector)
        {
            int length = vector.Count;
            double[] result = new double[length];

            foreach (var i in Enumerable.Range(0, length))
                result[i] = System.Math.Round(vector[i], 3);

            return result;
        }
    }
}