using MarkovApp.Configuration;
using MarkovApp.Models;
using MarkovApp.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Collections.ObjectModel;
using System.Windows;

namespace MarkovApp.Services
{
    public class GraphLogicService : IGraphLogicService
    {
        private readonly GraphSettings _settings;

        public GraphLogicService(IOptions<GraphSettings> settings)
        {
            _settings = settings.Value;
        }

        public int MaxNodes => _settings.MaxNodes;

        public bool CanAddNode(ObservableCollection<Node> nodes) => nodes.Count < MaxNodes;

        public Node AddNode(
            ObservableCollection<Node> nodes,
            ObservableCollection<InitialState> initialVector,
            Point position,
            ref int nodeCounter)
        {
            if (!CanAddNode(nodes))
                throw new InvalidOperationException($"Cannot add more than {MaxNodes} nodes.");

            var node = new Node(nodeCounter.ToString(), position);
            nodes.Add(node);
            initialVector.Add(new InitialState(initialVector.Count, 0));
            nodeCounter++;
            return node;
        }

        public void RemoveNode(
            ObservableCollection<Node> nodes,
            ObservableCollection<Edge> edges,
            ObservableCollection<Cell> matrix,
            ObservableCollection<InitialState> initialVector,
            Node nodeToRemove)
        {
            nodes.Remove(nodeToRemove);

            var connectedEdges = edges.Where(e => e.FromNode == nodeToRemove || e.ToNode == nodeToRemove).ToList();
            foreach (var edge in connectedEdges)
                edges.Remove(edge);

            matrix.Clear();
            initialVector.Clear();
        }

        public void AddEdge(ObservableCollection<Edge> edges, Node fromNode, Node toNode)
        {
            if (fromNode == toNode)
                return;

            if (!edges.Any(e => e.FromNode == fromNode && e.ToNode == toNode))
            {
                edges.Add(new Edge(fromNode, toNode));
                edges.Add(new Edge(toNode, fromNode));
            }
        }

        public void RemoveEdge(ObservableCollection<Edge> edges, Node fromNode, Node toNode)
        {
            var edge = edges.FirstOrDefault(e => e.FromNode == fromNode && e.ToNode == toNode);
            if (edge != null)
                edges.Remove(edge);

            var reverseEdge = edges.FirstOrDefault(e => e.FromNode == toNode && e.ToNode == fromNode);
            if (reverseEdge != null)
                edges.Remove(reverseEdge);
        }

        public void UpdateMatrixFromGraph(
            ObservableCollection<Node> nodes,
            ObservableCollection<Edge> edges,
            ObservableCollection<Cell> matrix,
            ObservableCollection<InitialState> initialVector)
        {
            matrix.Clear();
            var nodeIndexMap = nodes.Select((n, i) => (n, i)).ToDictionary(x => x.n.Id, x => x.i);

            foreach (var (fromNode, i) in nodes.Select((n, idx) => (n, idx)))
            {
                foreach (var (toNode, j) in nodes.Select((n, idx) => (n, idx)))
                {
                    double value = i == j ? fromNode.ProbabilityOfStaying :
                        edges.FirstOrDefault(e => e.FromNode == fromNode && e.ToNode == toNode)?.Value ?? 0;
                    matrix.Add(new Cell(i, j, value));
                }
            }

            if (initialVector != null)
            {
                initialVector.Clear();
                foreach (var (node, i) in nodes.Select((n, idx) => (n, idx)))
                    initialVector.Add(new InitialState(i, node.InitialProbability));
            }
        }
    }
}