namespace MarkovApp.Models
{
    public class AppState
    {
        public CalculationData? Data { get; set; }
        public List<Node>? Nodes { get; set; }
        public List<Edge>? Edges { get; set; }
    }
}
