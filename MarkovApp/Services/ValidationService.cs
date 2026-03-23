using MarkovApp.Configuration;
using MarkovApp.Models;
using MarkovApp.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace MarkovApp.Services
{
    public class ValidationService : IValidationService
    {
        private const double Tolerance = 1e-6;
        private const int MaxAllowed = 1_000_000;
        private readonly GraphSettings? _graphSettings;

        public ValidationService(IOptions<GraphSettings>? graphSettings = null)
        {
            _graphSettings = graphSettings?.Value;
        }

        public bool ValidateMaxIterations(int maxIterations, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (maxIterations <= 0)
            {
                errorMessage = "Maximum iterations must be a positive integer.";
                return false;
            }

            if (maxIterations > MaxAllowed)
            {
                errorMessage = $"Maximum iterations cannot exceed {MaxAllowed}.";
                return false;
            }

            return true;
        }
        public bool IsProbabilityValid(double value)
        {
            return value >= 0 && value <= 1;
        }

        public bool ValidateNode(Node node, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (!IsProbabilityValid(node.InitialProbability))
            {
                errorMessage = $"Initial probability of node {node.Id} must be between 0 and 1.";
                return false;
            }

            if (!IsProbabilityValid(node.ProbabilityOfStaying))
            {
                errorMessage = $"Probability of staying for node {node.Id} must be between 0 and 1.";
                return false;
            }

            return true;
        }

        public bool ValidateEdge(Edge edge, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (!IsProbabilityValid(edge.Value))
            {
                errorMessage = $"Edge from {edge.FromNode.Id} to {edge.ToNode.Id} must have value between 0 and 1.";
                return false;
            }

            return true;
        }

        private static bool ValidateTransitionMatrix(double[,] transitionMatrix, out string? errorMessage)
        {
            errorMessage = null;

            if (transitionMatrix == null || transitionMatrix.GetLength(0) == 0 || transitionMatrix.GetLength(1) == 0)
            {
                errorMessage = "Transition matrix cannot be empty.";
                return false;
            }

            int n = transitionMatrix.GetLength(0);
            foreach (var i in Enumerable.Range(0, n))
            {
                double rowSum = Enumerable.Range(0, n).Sum(j => transitionMatrix[i, j]);

                if (Math.Abs(rowSum - 1.0) > Tolerance)
                {
                    errorMessage = $"Row {i + 1} of the transition matrix does not sum to 1.";
                    return false;
                }
            }

            return true;
        }

        private static bool ValidateInitialStateVector(double[] initialStateVector, out string? errorMessage)
        {
            errorMessage = null;

            if (initialStateVector == null || initialStateVector.Length == 0)
            {
                errorMessage = "Initial state vector cannot be empty.";
                return false;
            }

            double sum = initialStateVector.Sum();
            if (Math.Abs(sum - 1.0) > Tolerance)
            {
                errorMessage = "Initial state vector does not sum to 1.";
                return false;
            }

            return true;
        }

        public bool ValidateCalculationData(CalculationData data, out string? errorMessage)
        {
            errorMessage = null;

            if (data == null)
            {
                errorMessage = "Calculation data cannot be null.";
                return false;
            }

            if (!ValidateTransitionMatrix(data.TransitionMatrix, out string? transitionMatrixError))
            {
                errorMessage = transitionMatrixError;
                return false;
            }

            if (!ValidateInitialStateVector(data.InitialStateVector, out string? initialStateVectorError))
            {
                errorMessage = initialStateVectorError;
                return false;
            }

            return true;
        }
        public string? ValidateAppState(AppState state)
        {
            if (state == null)
                return "AppState cannot be null.";

            if (_graphSettings != null && state.Nodes != null && state.Nodes.Count > _graphSettings.MaxNodes)
            {
                return $"The loaded state contains {state.Nodes.Count} nodes, but the maximum allowed is {_graphSettings.MaxNodes}.";
            }

            return null;
        }
    }
}