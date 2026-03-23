using FluentAssertions;
using MarkovApp.Models;
using System.Windows;

namespace MarkovApp.Tests.Models;

public class NodeTests
{
    public NodeTests()
    {
        TestHelper.EnsureConfigurationInitialized();
    }

    [Fact]
    public void Constructor_WithIdAndPosition_InitializesCorrectly()
    {
        // Arrange
        var id = "TestNode";
        var position = new Point(100, 200);

        // Act
        var node = new Node(id, position);

        // Assert
        node.Id.Should().Be(id);
        node.X.Should().Be(100);
        node.Y.Should().Be(200);
    }

    [Fact]
    public void Radius_IsConstant()
    {
        // Assert
        Node.Radius.Should().Be(20);
    }

    [Fact]
    public void X_PropertyChanged_RaisesEvent()
    {
        // Arrange
        var node = new Node("0", new Point(0, 0));
        var eventRaised = false;
        node.PropertyChanged += (_, e) => eventRaised = e.PropertyName == "X";

        // Act
        node.X = 100;

        // Assert
        eventRaised.Should().BeTrue();
        node.X.Should().Be(100);
    }

    [Fact]
    public void Y_PropertyChanged_RaisesEvent()
    {
        // Arrange
        var node = new Node("0", new Point(0, 0));
        var eventRaised = false;
        node.PropertyChanged += (_, e) => eventRaised = e.PropertyName == "Y";

        // Act
        node.Y = 200;

        // Assert
        eventRaised.Should().BeTrue();
        node.Y.Should().Be(200);
    }

    [Fact]
    public void InitialProbability_PropertyChanged_RaisesEvent()
    {
        // Arrange
        var node = new Node("0", new Point(0, 0));
        var eventRaised = false;
        node.PropertyChanged += (_, e) => eventRaised = e.PropertyName == "InitialProbability";

        // Act
        node.InitialProbability = 0.5;

        // Assert
        eventRaised.Should().BeTrue();
        node.InitialProbability.Should().Be(0.5);
    }

    [Fact]
    public void ProbabilityOfStaying_PropertyChanged_RaisesEvent()
    {
        // Arrange
        var node = new Node("0", new Point(0, 0));
        var eventRaised = false;
        node.PropertyChanged += (_, e) => eventRaised = e.PropertyName == "ProbabilityOfStaying";

        // Act
        node.ProbabilityOfStaying = 0.7;

        // Assert
        eventRaised.Should().BeTrue();
        node.ProbabilityOfStaying.Should().Be(0.7);
    }

    [Fact]
    public void IsAbsorbing_PropertyChanged_RaisesEvent()
    {
        // Arrange
        var node = new Node("0", new Point(0, 0));
        var eventRaised = false;
        node.PropertyChanged += (_, e) => eventRaised = e.PropertyName == "IsAbsorbing";

        // Act
        node.IsAbsorbing = true;

        // Assert
        eventRaised.Should().BeTrue();
        node.IsAbsorbing.Should().BeTrue();
    }

    [Fact]
    public void DefaultConstructor_InitializesWithDefaults()
    {
        // Act
        var node = new Node();

        // Assert
        node.Id.Should().BeEmpty();
        node.X.Should().Be(0);
        node.Y.Should().Be(0);
        node.InitialProbability.Should().Be(0);
        node.ProbabilityOfStaying.Should().Be(0);
        node.IsAbsorbing.Should().BeFalse();
    }
}
