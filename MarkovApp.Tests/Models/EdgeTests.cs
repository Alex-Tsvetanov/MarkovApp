using FluentAssertions;
using MarkovApp.Models;
using System.Windows;

namespace MarkovApp.Tests.Models;

public class EdgeTests
{
    [Fact]
    public void Constructor_WithNodes_InitializesCorrectly()
    {
        // Arrange
        var fromNode = new Node("0", new Point(0, 0));
        var toNode = new Node("1", new Point(100, 100));

        // Act
        var edge = new Edge(fromNode, toNode);

        // Assert
        edge.FromNode.Should().Be(fromNode);
        edge.ToNode.Should().Be(toNode);
        edge.Value.Should().Be(0);
    }

    [Fact]
    public void Value_PropertyChanged_RaisesEvent()
    {
        // Arrange
        var fromNode = new Node("0", new Point(0, 0));
        var toNode = new Node("1", new Point(100, 100));
        var edge = new Edge(fromNode, toNode);
        var eventRaised = false;
        edge.PropertyChanged += (_, e) => eventRaised = e.PropertyName == "Value";

        // Act
        edge.Value = 0.5;

        // Assert
        eventRaised.Should().BeTrue();
        edge.Value.Should().Be(0.5);
    }

    [Fact]
    public void DefaultConstructor_InitializesWithDefaults()
    {
        // Act
        var edge = new Edge();

        // Assert
        edge.FromNode.Should().BeNull();
        edge.ToNode.Should().BeNull();
        edge.Value.Should().Be(0);
    }

    [Fact]
    public void Value_CanBeSetMultipleTimes()
    {
        // Arrange
        var fromNode = new Node("0", new Point(0, 0));
        var toNode = new Node("1", new Point(100, 100));
        var edge = new Edge(fromNode, toNode);

        // Act
        edge.Value = 0.3;
        edge.Value = 0.7;
        edge.Value = 0.5;

        // Assert
        edge.Value.Should().Be(0.5);
    }

    [Fact]
    public void Edge_PreservesNodeReferences()
    {
        // Arrange
        var fromNode = new Node("0", new Point(50, 50));
        var toNode = new Node("1", new Point(150, 150));
        var edge = new Edge(fromNode, toNode);

        // Act
        fromNode.X = 100;
        toNode.Y = 200;

        // Assert
        edge.FromNode.X.Should().Be(100);
        edge.ToNode.Y.Should().Be(200);
    }
}
