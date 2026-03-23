using MarkovApp.Models;
using System.Collections.ObjectModel;
using System.Windows;

namespace MarkovApp.Services.Interfaces
{
    public interface IGraphLogicService
    {
        int MaxNodes { get; }

        bool CanAddNode(ObservableCollection<Node> nodes);

        Node AddNode(
            ObservableCollection<Node> nodes,
            ObservableCollection<InitialState> initialVector,
            Point position,
            ref int nodeCounter);

        void RemoveNode(
            ObservableCollection<Node> nodes,
            ObservableCollection<Edge> edges,
            ObservableCollection<Cell> matrix,
            ObservableCollection<InitialState> initialVector,
            Node nodeToRemove);

        void AddEdge(
            ObservableCollection<Edge> edges,
            Node fromNode,
            Node toNode);

        void RemoveEdge(
            ObservableCollection<Edge> edges,
            Node fromNode,
            Node toNode);

        void UpdateMatrixFromGraph(
            ObservableCollection<Node> nodes,
            ObservableCollection<Edge> edges,
            ObservableCollection<Cell> matrix,
            ObservableCollection<InitialState> initialVector);
    }
}
