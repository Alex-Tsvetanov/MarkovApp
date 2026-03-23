using FluentAssertions;
using MarkovApp.Utilities;
using MarkovApp.Models;
using System.Collections.ObjectModel;
using System.Windows;

namespace MarkovApp.Tests.Utilities;

public class GraphGeometryHelperTests
{
    public GraphGeometryHelperTests()
    {
        TestHelper.EnsureConfigurationInitialized();
    }

    [Fact]
    public void IsPositionValid_WithValidPosition_ReturnsTrue()
    {
        // Arrange
        var nodes = new ObservableCollection<Node>();
        var edges = new ObservableCollection<Edge>();
        var position = new Point(100, 100);

        // Act
        var result = GraphGeometryHelper.IsPositionValid(position, nodes, edges, 50, 50);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsPositionValid_TooCloseToNode_ReturnsFalse()
    {
        // Arrange
        var nodes = new ObservableCollection<Node>
        {
            new Node("0", new Point(100, 100))
        };
        var edges = new ObservableCollection<Edge>();
        var position = new Point(105, 105); // Very close to existing node

        // Act
        var result = GraphGeometryHelper.IsPositionValid(position, nodes, edges, 50, 50);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsPositionValid_TooCloseToLeftEdge_ReturnsTrue()
    {
        // Arrange - GraphGeometryHelper doesn't check canvas margins
        var nodes = new ObservableCollection<Node>();
        var edges = new ObservableCollection<Edge>();
        var position = new Point(10, 100);

        // Act
        var result = GraphGeometryHelper.IsPositionValid(position, nodes, edges, 50, 50);

        // Assert
        result.Should().BeTrue(); // No margin validation in GraphGeometryHelper
    }

    [Fact]
    public void IsPositionValid_TooCloseToTopEdge_ReturnsTrue()
    {
        // Arrange - GraphGeometryHelper doesn't check canvas margins
        var nodes = new ObservableCollection<Node>();
        var edges = new ObservableCollection<Edge>();
        var position = new Point(100, 10);

        // Act
        var result = GraphGeometryHelper.IsPositionValid(position, nodes, edges, 50, 50);

        // Assert
        result.Should().BeTrue(); // No margin validation in GraphGeometryHelper
    }

    [Fact]
    public void IsPositionValid_ExactlyAtMinimumDistance_ReturnsTrue()
    {
        // Arrange
        var nodes = new ObservableCollection<Node>
        {
            new Node("0", new Point(100, 100))
        };
        var edges = new ObservableCollection<Edge>();
        var position = new Point(200, 100); // Exactly at minimum distance

        // Act
        var result = GraphGeometryHelper.IsPositionValid(position, nodes, edges, 50, 50);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsPositionValid_WithMultipleNodes_ChecksAllDistances()
    {
        // Arrange
        var nodes = new ObservableCollection<Node>
        {
            new Node("0", new Point(100, 100)),
            new Node("1", new Point(300, 100)),
            new Node("2", new Point(200, 250))
        };
        var edges = new ObservableCollection<Edge>();
        var validPosition = new Point(400, 300);
        var invalidPosition = new Point(195, 255); // Too close to node 2

        // Act
        var validResult = GraphGeometryHelper.IsPositionValid(validPosition, nodes, edges, 50, 50);
        var invalidResult = GraphGeometryHelper.IsPositionValid(invalidPosition, nodes, edges, 50, 50);

        // Assert
        validResult.Should().BeTrue();
        invalidResult.Should().BeFalse();
    }

    [Fact]
    public void IsPositionValid_WithEmptyCollections_ReturnsTrue()
    {
        // Arrange - With no nodes or edges, any position is valid
        var nodes = new ObservableCollection<Node>();
        var edges = new ObservableCollection<Edge>();
        var position1 = new Point(100, 100);
        var position2 = new Point(5, 100);

        // Act
        var result1 = GraphGeometryHelper.IsPositionValid(position1, nodes, edges, 50, 50);
        var result2 = GraphGeometryHelper.IsPositionValid(position2, nodes, edges, 50, 50);

        // Assert
        result1.Should().BeTrue();
        result2.Should().BeTrue(); // No margin checking
    }
}
