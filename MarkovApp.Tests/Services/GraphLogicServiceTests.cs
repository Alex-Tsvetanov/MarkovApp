using FluentAssertions;
using MarkovApp.Configuration;
using MarkovApp.Models;
using MarkovApp.Services;
using Microsoft.Extensions.Options;
using System.Collections.ObjectModel;
using System.Windows;

namespace MarkovApp.Tests.Services;

public class GraphLogicServiceTests
{
    private readonly GraphLogicService _service;

    public GraphLogicServiceTests()
    {
        var settings = Options.Create(new GraphSettings
        {
            MaxNodes = 12,
            NodeRadius = 20.0,
            MinNodeDistance = 50.0,
            MinEdgeDistance = 50.0
        });
        _service = new GraphLogicService(settings);
    }

    [Fact]
    public void MaxNodes_ShouldBe12()
    {
        // Assert
        _service.MaxNodes.Should().Be(12);
    }

    [Fact]
    public void CanAddNode_WithLessThanMaxNodes_ReturnsTrue()
    {
        // Arrange
        var nodes = new ObservableCollection<Node>
        {
            new Node("0", new Point(100, 100)),
            new Node("1", new Point(200, 200))
        };

        // Act
        var result = _service.CanAddNode(nodes);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanAddNode_WithMaxNodes_ReturnsFalse()
    {
        // Arrange
        var nodes = new ObservableCollection<Node>();
        for (int i = 0; i < 12; i++)
            nodes.Add(new Node(i.ToString(), new Point(i * 50, i * 50)));

        // Act
        var result = _service.CanAddNode(nodes);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void AddNode_WithValidInput_AddsNodeAndInitialState()
    {
        // Arrange
        var nodes = new ObservableCollection<Node>();
        var initialVector = new ObservableCollection<InitialState>();
        var position = new Point(100, 150);
        int counter = 0;

        // Act
        var node = _service.AddNode(nodes, initialVector, position, ref counter);

        // Assert
        nodes.Should().HaveCount(1);
        node.Id.Should().Be("0");
        node.X.Should().Be(100);
        node.Y.Should().Be(150);
        initialVector.Should().HaveCount(1);
        initialVector[0].Index.Should().Be(0);
        counter.Should().Be(1);
    }

    [Fact]
    public void AddNode_ExceedingMaxNodes_ThrowsInvalidOperationException()
    {
        // Arrange
        var nodes = new ObservableCollection<Node>();
        for (int i = 0; i < 12; i++)
            nodes.Add(new Node(i.ToString(), new Point(i * 50, i * 50)));
        var initialVector = new ObservableCollection<InitialState>();
        int counter = 12;

        // Act
        Action act = () => _service.AddNode(nodes, initialVector, new Point(100, 100), ref counter);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*12*");
    }

    [Fact]
    public void RemoveNode_RemovesNodeAndConnectedEdges()
    {
        // Arrange
        var node1 = new Node("0", new Point(100, 100));
        var node2 = new Node("1", new Point(200, 200));
        var node3 = new Node("2", new Point(300, 300));
        var nodes = new ObservableCollection<Node> { node1, node2, node3 };
        var edges = new ObservableCollection<Edge>
        {
            new Edge(node1, node2),
            new Edge(node2, node3),
            new Edge(node1, node3)
        };
        var matrix = new ObservableCollection<Cell>();
        var initialVector = new ObservableCollection<InitialState>();

        // Act
        _service.RemoveNode(nodes, edges, matrix, initialVector, node2);

        // Assert
        nodes.Should().HaveCount(2);
        nodes.Should().NotContain(node2);
        edges.Should().HaveCount(1);
        edges.Should().NotContain(e => e.FromNode == node2 || e.ToNode == node2);
    }

    [Fact]
    public void AddEdge_WithDifferentNodes_AddsBidirectionalEdges()
    {
        // Arrange
        var node1 = new Node("0", new Point(100, 100));
        var node2 = new Node("1", new Point(200, 200));
        var edges = new ObservableCollection<Edge>();

        // Act
        _service.AddEdge(edges, node1, node2);

        // Assert
        edges.Should().HaveCount(2);
        edges.Should().Contain(e => e.FromNode == node1 && e.ToNode == node2);
        edges.Should().Contain(e => e.FromNode == node2 && e.ToNode == node1);
    }

    [Fact]
    public void AddEdge_WithSameNode_DoesNotAddEdge()
    {
        // Arrange
        var node1 = new Node("0", new Point(100, 100));
        var edges = new ObservableCollection<Edge>();

        // Act
        _service.AddEdge(edges, node1, node1);

        // Assert
        edges.Should().BeEmpty();
    }

    [Fact]
    public void AddEdge_WhenEdgeAlreadyExists_DoesNotAddDuplicate()
    {
        // Arrange
        var node1 = new Node("0", new Point(100, 100));
        var node2 = new Node("1", new Point(200, 200));
        var edges = new ObservableCollection<Edge>
        {
            new Edge(node1, node2),
            new Edge(node2, node1)
        };

        // Act
        _service.AddEdge(edges, node1, node2);

        // Assert
        edges.Should().HaveCount(2);
    }

    [Fact]
    public void RemoveEdge_RemovesBidirectionalEdges()
    {
        // Arrange
        var node1 = new Node("0", new Point(100, 100));
        var node2 = new Node("1", new Point(200, 200));
        var edges = new ObservableCollection<Edge>
        {
            new Edge(node1, node2),
            new Edge(node2, node1)
        };

        // Act
        _service.RemoveEdge(edges, node1, node2);

        // Assert
        edges.Should().BeEmpty();
    }

    [Fact]
    public void UpdateMatrixFromGraph_CreatesCorrectTransitionMatrix()
    {
        // Arrange
        var node1 = new Node("0", new Point(100, 100)) { ProbabilityOfStaying = 0.5, InitialProbability = 0.6 };
        var node2 = new Node("1", new Point(200, 200)) { ProbabilityOfStaying = 0.3, InitialProbability = 0.4 };
        var nodes = new ObservableCollection<Node> { node1, node2 };
        var edge = new Edge(node1, node2) { Value = 0.5 };
        var edges = new ObservableCollection<Edge> { edge };
        var matrix = new ObservableCollection<Cell>();
        var initialVector = new ObservableCollection<InitialState>();

        // Act
        _service.UpdateMatrixFromGraph(nodes, edges, matrix, initialVector);

        // Assert
        matrix.Should().HaveCount(4); // 2x2 matrix
        matrix.Should().Contain(c => c.Row == 0 && c.Column == 0 && c.Value == 0.5);
        matrix.Should().Contain(c => c.Row == 0 && c.Column == 1 && c.Value == 0.5);
        matrix.Should().Contain(c => c.Row == 1 && c.Column == 0 && c.Value == 0.0);
        matrix.Should().Contain(c => c.Row == 1 && c.Column == 1 && c.Value == 0.3);

        initialVector.Should().HaveCount(2);
        initialVector[0].TryGetValue(out var val0).Should().BeTrue();
        val0.Should().Be(0.6);
        initialVector[1].TryGetValue(out var val1).Should().BeTrue();
        val1.Should().Be(0.4);
    }

    [Fact]
    public void UpdateMatrixFromGraph_WithNullInitialVector_DoesNotUpdateVector()
    {
        // Arrange
        var node1 = new Node("0", new Point(100, 100)) { ProbabilityOfStaying = 0.5 };
        var nodes = new ObservableCollection<Node> { node1 };
        var edges = new ObservableCollection<Edge>();
        var matrix = new ObservableCollection<Cell>();

        // Act
        _service.UpdateMatrixFromGraph(nodes, edges, matrix, null!);

        // Assert
        matrix.Should().HaveCount(1);
        // Should not throw, just skip initial vector update
    }
}
