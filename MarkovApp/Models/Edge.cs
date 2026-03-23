using MarkovApp.Infrastructure;

namespace MarkovApp.Models
{
    public class Edge : ObservableObject
    {
        public Node FromNode { get; set; } = null!;
        public Node ToNode { get; set; } = null!;

        private double _value;
        public double Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }
        public Edge() { }
        public Edge(Node from, Node to)
        {
            FromNode = from;
            ToNode = to;
        }
    }
}