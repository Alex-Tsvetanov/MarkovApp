using FluentAssertions;
using MarkovApp.Models;
using MarkovApp.Services;
using System.Collections.ObjectModel;

namespace MarkovApp.Tests.Services;

public class GraphDataServiceTests
{
    private readonly GraphDataService _service;

    public GraphDataServiceTests()
    {
        _service = new GraphDataService();
    }

    [Fact]
    public void ToCalculationData_WithValidData_ReturnsCorrectData()
    {
        // Arrange
        var matrix = new ObservableCollection<Cell>
        {
            new Cell(0, 0, 0.7),
            new Cell(0, 1, 0.3),
            new Cell(1, 0, 0.4),
            new Cell(1, 1, 0.6)
        };
        var initialVector = new ObservableCollection<InitialState>
        {
            new InitialState(0, 1.0),
            new InitialState(1, 0.0)
        };

        // Act
        var result = _service.ToCalculationData(matrix, initialVector, 2);

        // Assert
        result.Should().NotBeNull();
        result.TransitionMatrix.GetLength(0).Should().Be(2);
        result.TransitionMatrix.GetLength(1).Should().Be(2);
        result.TransitionMatrix[0, 0].Should().Be(0.7);
        result.TransitionMatrix[0, 1].Should().Be(0.3);
        result.TransitionMatrix[1, 0].Should().Be(0.4);
        result.TransitionMatrix[1, 1].Should().Be(0.6);
        result.InitialStateVector[0].Should().Be(1.0);
        result.InitialStateVector[1].Should().Be(0.0);
    }

    [Fact]
    public void ToCalculationData_WithNullMatrix_ThrowsArgumentNullException()
    {
        // Arrange
        var initialVector = new ObservableCollection<InitialState>();

        // Act
        Action act = () => _service.ToCalculationData(null!, initialVector, 2);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ToCalculationData_WithNullInitialVector_ThrowsArgumentNullException()
    {
        // Arrange
        var matrix = new ObservableCollection<Cell>();

        // Act
        Action act = () => _service.ToCalculationData(matrix, null!, 2);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void SyncCalculationData_UpdatesCollections()
    {
        // Arrange
        var nodes = new ObservableCollection<Node>();
        var edges = new ObservableCollection<Edge>();
        var matrix = new ObservableCollection<Cell>();
        var initialVector = new ObservableCollection<InitialState>();

        var transitionMatrix = new double[,]
        {
            { 0.5, 0.5 },
            { 0.3, 0.7 }
        };
        var stateVector = new double[] { 0.6, 0.4 };
        var data = new CalculationData(transitionMatrix, stateVector);

        // Act
        _service.SyncCalculationData(nodes, edges, matrix, initialVector, data);

        // Assert
        matrix.Should().HaveCount(4);
        initialVector.Should().HaveCount(2);
        initialVector[0].TryGetValue(out var val0).Should().BeTrue();
        val0.Should().Be(0.6);
        initialVector[1].TryGetValue(out var val1).Should().BeTrue();
        val1.Should().Be(0.4);
    }

    [Fact]
    public void SyncCalculationData_ClearsPreviousData()
    {
        // Arrange
        var nodes = new ObservableCollection<Node> { new Node("0", new System.Windows.Point(0, 0)) };
        var edges = new ObservableCollection<Edge>();
        var matrix = new ObservableCollection<Cell> { new Cell(0, 0, 0.5) };
        var initialVector = new ObservableCollection<InitialState> { new InitialState(0, 1.0) };

        var transitionMatrix = new double[,] { { 1.0 } };
        var stateVector = new double[] { 1.0 };
        var data = new CalculationData(transitionMatrix, stateVector);

        // Act
        _service.SyncCalculationData(nodes, edges, matrix, initialVector, data);

        // Assert
        nodes.Should().BeEmpty();
        edges.Should().BeEmpty();
        matrix.Should().HaveCount(1);
        initialVector.Should().HaveCount(1);
    }

    [Fact]
    public void SyncCalculationData_WithNullData_ThrowsArgumentNullException()
    {
        // Arrange
        var nodes = new ObservableCollection<Node>();
        var edges = new ObservableCollection<Edge>();
        var matrix = new ObservableCollection<Cell>();
        var initialVector = new ObservableCollection<InitialState>();

        // Act
        Action act = () => _service.SyncCalculationData(nodes, edges, matrix, initialVector, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }
}
