using MarkovApp.Infrastructure;
using System.Windows;

namespace MarkovApp.Models
{
    public class Node : ObservableObject
    {
        public string Id { get; set; } = string.Empty;
        public static double Radius => AppConfig.Graph.NodeRadius;

        private double _x;
        public double X
        {
            get => _x;
            set => SetProperty(ref _x, value);
        }

        private double _y;
        public double Y
        {
            get => _y;
            set => SetProperty(ref _y, value);
        }

        private double _initialProbability;
        public double InitialProbability
        {
            get => _initialProbability;
            set => SetProperty(ref _initialProbability, value);
        }

        private double _probabilityOfStaying;
        public double ProbabilityOfStaying
        {
            get => _probabilityOfStaying;
            set => SetProperty(ref _probabilityOfStaying, value);
        }

        private bool _isAbsorbing;
        public bool IsAbsorbing
        {
            get => _isAbsorbing;
            set => SetProperty(ref _isAbsorbing, value);
        }
        public Node() { }
        public Node(string id, Point position)
        {
            Id = id;
            X = position.X;
            Y = position.Y;
        }
    }
}