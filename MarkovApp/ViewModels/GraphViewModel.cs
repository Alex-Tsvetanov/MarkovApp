using MarkovApp.Infrastructure;
using MarkovApp.Models;
using MarkovApp.Services.Interfaces;
using MarkovApp.Utilities;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace MarkovApp.ViewModels
{
    public class GraphViewModel : ObservableObject
    {
        private readonly IGraphLogicService _graphLogicService;
        private readonly IGraphDataService _graphService;
        private readonly IDialogService _dialogService;
        private readonly IValidationService _validationService;

        private bool _isDrawingOverride = false;
        private bool _isManualOverride = false;

        private int _nodeCounter = 0;
        private Node? _edgeStartNode;
        private Point? _tempLineEnd;

        public bool IsManualOverride
        {
            get => _isManualOverride;
            set => _isManualOverride = value;
        }

        public bool IsDrawingOverride
        {
            get => _isDrawingOverride;
            set => _isDrawingOverride = value;
        }

        public ObservableCollection<Node> Nodes { get; } = new();
        public ObservableCollection<Edge> Edges { get; } = new();
        public ObservableCollection<Cell> TransitionMatrix { get; } = new();
        public ObservableCollection<InitialState> InitialStateVector { get; } = new();

        public Node? EdgeStartNode
        {
            get => _edgeStartNode;
            private set => SetProperty(ref _edgeStartNode, value);
        }

        public Point? TempLineEnd
        {
            get => _tempLineEnd;
            set => SetProperty(ref _tempLineEnd, value);
        }

        public Visibility TempLineVisibility =>
            (EdgeStartNode != null && TempLineEnd != null) ? Visibility.Visible : Visibility.Collapsed;

        public Point? TempLineStart =>
            EdgeStartNode == null ? null : new Point(EdgeStartNode.X + Node.Radius, EdgeStartNode.Y + Node.Radius);

        public ICommand CanvasLeftClickCommand { get; }
        public ICommand CanvasMouseMoveCommand { get; }
        public ICommand CanvasRightReleaseCommand { get; }
        public ICommand NodeLeftClickCommand { get; }
        public ICommand NodeRightClickCommand { get; }
        public ICommand EdgeLeftClickCommand { get; }

        public GraphViewModel(
            IGraphLogicService graphLogic,
            IGraphDataService graphService,
            IDialogService dialogService,
            IValidationService validationService)
        {
            _graphLogicService = graphLogic;
            _graphService = graphService;
            _dialogService = dialogService;
            _validationService = validationService;

            CanvasLeftClickCommand = new RelayCommand<MouseButtonEventArgs>(OnCanvasLeftClick);
            CanvasMouseMoveCommand = new RelayCommand<MouseEventArgs>(OnCanvasMouseMove);
            CanvasRightReleaseCommand = new RelayCommand<MouseButtonEventArgs>(OnCanvasRightRelease);
            NodeLeftClickCommand = new RelayCommand<Node>(EditNode);
            NodeRightClickCommand = new RelayCommand<Node>(StartEdge);
            EdgeLeftClickCommand = new RelayCommand<Edge>(EditEdge);
        }

        public void SetManualMode()
        {
            IsManualOverride = true;
            IsDrawingOverride = false;
        }

        public void ResetOverrides()
        {
            IsManualOverride = false;
            IsDrawingOverride = false;
        }

        private void AddNode(Point position)
        {
            if (!_graphLogicService.CanAddNode(Nodes))
            {
                _dialogService.ShowError($"Cannot add more than {_graphLogicService.MaxNodes} nodes.");
                return;
            }

            if (IsManualOverride && !IsDrawingOverride)
            {
                TransitionMatrix.Clear();
                InitialStateVector.Clear();
            }

            IsDrawingOverride = true;
            IsManualOverride = false;

            _graphLogicService.AddNode(Nodes, InitialStateVector, position, ref _nodeCounter);

            _graphLogicService.UpdateMatrixFromGraph(Nodes, Edges, TransitionMatrix, InitialStateVector);
        }

        private void StartEdge(Node fromNode)
        {
            EdgeStartNode = fromNode;
            TempLineEnd = new Point(fromNode.X + Node.Radius, fromNode.Y + Node.Radius);
            OnPropertyChanged(nameof(TempLineStart));
            OnPropertyChanged(nameof(TempLineVisibility));
        }

        private void UpdateTempEdge(Point current)
        {
            if (EdgeStartNode != null)
                TempLineEnd = current;
        }

        private void EndEdge(Node? toNode)
        {
            if (EdgeStartNode == null || toNode == null || EdgeStartNode == toNode)
            {
                ClearTempEdge();
                return;
            }

            _graphLogicService.AddEdge(Edges, EdgeStartNode, toNode);
            ClearTempEdge();
            _graphLogicService.UpdateMatrixFromGraph(Nodes, Edges, TransitionMatrix, InitialStateVector);
        }

        private void ClearTempEdge()
        {
            EdgeStartNode = null;
            TempLineEnd = null;
            OnPropertyChanged(nameof(TempLineStart));
            OnPropertyChanged(nameof(TempLineVisibility));
        }

        private void OnCanvasLeftClick(MouseButtonEventArgs e)
        {
            var pos = e.GetPosition((IInputElement)e.Source);
            if (GraphGeometryHelper.IsPositionValid(pos, Nodes, Edges, AppConfig.Graph.MinNodeDistance, AppConfig.Graph.MinEdgeDistance))
                AddNode(pos);
        }

        private void OnCanvasRightRelease(MouseButtonEventArgs e)
        {
            var pos = e.GetPosition((IInputElement)e.Source);
            var node = Nodes.FirstOrDefault(n =>
                (n.X + Node.Radius - pos.X) * (n.X + Node.Radius - pos.X) +
                (n.Y + Node.Radius - pos.Y) * (n.Y + Node.Radius - pos.Y)
                < Node.Radius * Node.Radius);
            EndEdge(node);
        }

        private void OnCanvasMouseMove(MouseEventArgs e)
        {
            if (EdgeStartNode != null && e.RightButton == MouseButtonState.Pressed)
            {
                var pos = e.GetPosition((IInputElement)e.Source);
                UpdateTempEdge(pos);
            }
        }

        private void EditNode(Node node)
        {
            var nodeVM = new NodePropertiesViewModel(node, _validationService, _dialogService);
            if (_dialogService.ShowNodeProperties(nodeVM) == true)
                _graphLogicService.UpdateMatrixFromGraph(Nodes, Edges, TransitionMatrix, InitialStateVector);
        }

        private void EditEdge(Edge edge)
        {
            var edgeVM = new EdgePropertiesViewModel(edge, _validationService, _dialogService);
            if (_dialogService.ShowEdgeProperties(edgeVM) == true)
                _graphLogicService.UpdateMatrixFromGraph(Nodes, Edges, TransitionMatrix, InitialStateVector);
        }

        public CalculationData ToCalculationData()
        {
            return _graphService.ToCalculationData(TransitionMatrix, InitialStateVector, Nodes.Count);
        }

        public void SyncCalculationData(CalculationData data)
        {
            _graphService.SyncCalculationData(Nodes, Edges, TransitionMatrix, InitialStateVector, data);
        }

        public AppState ToAppState(bool includeGraph = true)
        {
            var state = new AppState { Data = ToCalculationData() };

            if (!includeGraph) return state;

            state.Nodes = Nodes.Select(n => new Node(n.Id, new Point(n.X, n.Y))
            {
                InitialProbability = n.InitialProbability,
                ProbabilityOfStaying = n.ProbabilityOfStaying,
                IsAbsorbing = n.IsAbsorbing
            }).ToList();

            state.Edges = Edges.Select(e => new Edge(e.FromNode, e.ToNode) { Value = e.Value }).ToList();

            return state;
        }

        public void LoadFromAppState(AppState state)
        {
            Nodes.Clear();
            foreach (var n in state.Nodes ?? Enumerable.Empty<Node>())
            {
                Nodes.Add(new Node(n.Id, new Point(n.X, n.Y))
                {
                    InitialProbability = n.InitialProbability,
                    ProbabilityOfStaying = n.ProbabilityOfStaying,
                    IsAbsorbing = n.IsAbsorbing
                });
            }

            Edges.Clear();
            foreach (var e in state.Edges ?? Enumerable.Empty<Edge>())
            {
                var fromNode = Nodes.FirstOrDefault(n => n.Id == e.FromNode.Id);
                var toNode = Nodes.FirstOrDefault(n => n.Id == e.ToNode.Id);
                if (fromNode != null && toNode != null)
                    Edges.Add(new Edge(fromNode, toNode) { Value = e.Value });
            }

            _graphLogicService.UpdateMatrixFromGraph(Nodes, Edges, TransitionMatrix, InitialStateVector);
            _nodeCounter = Nodes.Count;
        }

        public void ResetNodeCounter() => _nodeCounter = 0;
    }
}