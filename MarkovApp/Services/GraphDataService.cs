using MarkovApp.Models;
using MarkovApp.Services.Interfaces;
using System.Collections.ObjectModel;

namespace MarkovApp.Services
{
    public class GraphDataService : IGraphDataService
    {
        public void SyncCalculationData(
    ObservableCollection<Node> nodes,
    ObservableCollection<Edge> edges,
    ObservableCollection<Cell> transitionMatrix,
    ObservableCollection<InitialState> initialVector,
    CalculationData data)
        {
            ArgumentNullException.ThrowIfNull(nodes);
            ArgumentNullException.ThrowIfNull(edges);
            ArgumentNullException.ThrowIfNull(transitionMatrix);
            ArgumentNullException.ThrowIfNull(initialVector);
            ArgumentNullException.ThrowIfNull(data);

            int newSize = data.InitialStateVector.Length;

            nodes.Clear();
            edges.Clear();
            transitionMatrix.Clear();
            initialVector.Clear();

            foreach (var i in Enumerable.Range(0, newSize))
                initialVector.Add(new InitialState(i, data.InitialStateVector[i]));

            foreach (var i in Enumerable.Range(0, newSize))
                foreach (var j in Enumerable.Range(0, newSize))
                    transitionMatrix.Add(new Cell(i, j, data.TransitionMatrix[i, j]));
        }

        public CalculationData ToCalculationData(
    ObservableCollection<Cell> transitionMatrix,
    ObservableCollection<InitialState> initialStateVector,
    int nodeCount)
        {
            ArgumentNullException.ThrowIfNull(transitionMatrix);
            ArgumentNullException.ThrowIfNull(initialStateVector);

            double[,] m = new double[nodeCount, nodeCount];
            double[] v = new double[nodeCount];

            foreach (var i in Enumerable.Range(0, nodeCount))
                v[i] = initialStateVector[i].TryGetValue(out var val) ? val : 0;

            foreach (Cell cell in transitionMatrix)
            {
                m[cell.Row, cell.Column] = cell.Value;
            }

            return new CalculationData(m, v);
        }

        public void UpdateMatrixFromGraph(
    ObservableCollection<Node> nodes,
    ObservableCollection<Edge> edges,
    ObservableCollection<Cell> transitionMatrix,
    ObservableCollection<InitialState>? initialStateVector = null)
        {
            transitionMatrix.Clear();

            var nodeIndexMap = nodes.Select((n, i) => (n.Id, i)).ToDictionary(x => x.Id, x => x.i);

            foreach (var (fromNode, i) in nodes.Select((n, idx) => (n, idx)))
            {
                int row = nodeIndexMap[fromNode.Id];

                foreach (var (toNode, j) in nodes.Select((n, idx) => (n, idx)))
                {
                    int col = nodeIndexMap[toNode.Id];
                    double value = row == col ? fromNode.ProbabilityOfStaying : edges.FirstOrDefault(e => e.FromNode == fromNode && e.ToNode == toNode)?.Value ?? 0;
                    transitionMatrix.Add(new Cell(row, col, value));
                }
            }

            if (initialStateVector != null)
            {
                initialStateVector.Clear();
                foreach (var (node, i) in nodes.Select((n, idx) => (n, idx)))
                    initialStateVector.Add(new InitialState(i, node.InitialProbability));
            }
        }
    }
}