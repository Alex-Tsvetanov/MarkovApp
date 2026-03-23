using MarkovApp.Models;
using System.Collections.ObjectModel;

namespace MarkovApp.Services.Interfaces
{
    public interface IGraphDataService
    {
        void SyncCalculationData(
            ObservableCollection<Node> nodes,
            ObservableCollection<Edge> edges,
            ObservableCollection<Cell> transitionMatrix,
            ObservableCollection<InitialState> initialStateVector,
            CalculationData manualData);

        CalculationData ToCalculationData(
            ObservableCollection<Cell> transitionMatrix,
            ObservableCollection<InitialState> initialStateVector,
            int nodeCount);

        void UpdateMatrixFromGraph(
            ObservableCollection<Node> nodes,
            ObservableCollection<Edge> edges,
            ObservableCollection<Cell> transitionMatrix,
            ObservableCollection<InitialState>? initialStateVector);
    }
}
