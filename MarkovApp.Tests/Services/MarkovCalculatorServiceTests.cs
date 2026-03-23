using FluentAssertions;
using MarkovApp.Configuration;
using MarkovApp.Models;
using MarkovApp.Services;
using Microsoft.Extensions.Options;

namespace MarkovApp.Tests.Services;

public class MarkovCalculatorServiceTests
{
    private readonly MarkovCalculatorService _service;

    public MarkovCalculatorServiceTests()
    {
        var settings = Options.Create(new CalculationSettings
        {
            DefaultMaxIterations = 1000,
            ConvergenceEpsilon = 0.0001
        });
        _service = new MarkovCalculatorService(settings);
    }

    [Fact]
    public void CalculateRegularChain_WithValidData_ReturnsResult()
    {
        // Arrange
        var transitionMatrix = new double[,]
        {
            { 0.7, 0.3 },
            { 0.4, 0.6 }
        };
        var initialState = new double[] { 1.0, 0.0 };
        var data = new CalculationData(transitionMatrix, initialState);

        // Act
        var result = _service.CalculateRegularChain(data, 1000);

        // Assert
        result.Should().NotBeNull();
        result.LimitingProbabilities.Should().NotBeNull();
        result.LimitingProbabilities.Length.Should().Be(2);
        result.ResultVector.Should().NotBeNull();
        result.TransitionMatrix.Should().NotBeNull();
    }

    [Fact]
    public void CalculateRegularChain_WithConvergence_ReturnsStableProbabilities()
    {
        // Arrange: Simple 2-state regular Markov chain
        var transitionMatrix = new double[,]
        {
            { 0.7, 0.3 },
            { 0.4, 0.6 }
        };
        var initialState = new double[] { 1.0, 0.0 };
        var data = new CalculationData(transitionMatrix, initialState);

        // Act
        var result = _service.CalculateRegularChain(data, 1000);

        // Assert
        result.LimitingProbabilities[0].Should().BeApproximately(0.571, 0.01);
        result.LimitingProbabilities[1].Should().BeApproximately(0.429, 0.01);
    }

    [Fact]
    public void CalculateRegularChain_WithNullData_ThrowsArgumentNullException()
    {
        // Act
        Action act = () => _service.CalculateRegularChain(null!, 100);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CalculateRegularChain_WithNullMatrix_ThrowsArgumentNullException()
    {
        // Arrange - CalculationData constructor throws ArgumentNullException for null matrix
        // Act & Assert
        Action act = () => new CalculationData(null!, new double[] { 1.0 });

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CalculateRegularChain_WithNonSquareMatrix_ThrowsArgumentException()
    {
        // Arrange
        var transitionMatrix = new double[,]
        {
            { 0.7, 0.3, 0.0 },
            { 0.4, 0.6, 0.0 }
        };
        var initialState = new double[] { 1.0, 0.0 };
        var data = new CalculationData(transitionMatrix, initialState);

        // Act
        Action act = () => _service.CalculateRegularChain(data, 100);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*square*");
    }

    [Fact]
    public void CalculateRegularChain_WithMismatchedVectorSize_ThrowsArgumentException()
    {
        // Arrange
        var transitionMatrix = new double[,]
        {
            { 0.7, 0.3 },
            { 0.4, 0.6 }
        };
        var initialState = new double[] { 1.0, 0.0, 0.0 };
        var data = new CalculationData(transitionMatrix, initialState);

        // Act
        Action act = () => _service.CalculateRegularChain(data, 100);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*length*match*");
    }

    [Fact]
    public void CalculateAbsorbingChain_WithValidAbsorbingState_ReturnsResult()
    {
        // Arrange: 3-state chain with state 2 absorbing
        var transitionMatrix = new double[,]
        {
            { 0.5, 0.3, 0.2 },
            { 0.0, 0.6, 0.4 },
            { 0.0, 0.0, 1.0 }
        };
        var initialState = new double[] { 1.0, 0.0, 0.0 };
        var data = new CalculationData(transitionMatrix, initialState);

        // Act
        var result = _service.CalculateAbsorbingChain(data);

        // Assert
        result.Should().NotBeNull();
        result.AverageTransitions.Should().NotBeNull();
        result.Probabilities.Should().NotBeNull();
        result.AbsorbingStates.Should().Contain(2);
    }

    [Fact]
    public void CalculateAbsorbingChain_WithNullData_ThrowsArgumentNullException()
    {
        // Act
        Action act = () => _service.CalculateAbsorbingChain(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CalculateAbsorbingChain_WithAllAbsorbingStates_ReturnsEmptyMatrices()
    {
        // Arrange: All states absorbing
        var transitionMatrix = new double[,]
        {
            { 1.0, 0.0 },
            { 0.0, 1.0 }
        };
        var initialState = new double[] { 0.5, 0.5 };
        var data = new CalculationData(transitionMatrix, initialState);

        // Act
        var result = _service.CalculateAbsorbingChain(data);

        // Assert
        result.AverageTransitions.GetLength(0).Should().Be(0);
        result.Probabilities.GetLength(0).Should().Be(0);
        result.AbsorbingStates.Should().HaveCount(2);
    }

    [Fact]
    public void IsAbsorbingMatrix_WithAbsorbingState_ReturnsTrue()
    {
        // Arrange
        var matrix = new double[,]
        {
            { 0.5, 0.5, 0.0 },
            { 0.3, 0.4, 0.3 },
            { 0.0, 0.0, 1.0 }
        };

        // Act
        var result = _service.IsAbsorbingMatrix(matrix);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsAbsorbingMatrix_WithoutAbsorbingState_ReturnsFalse()
    {
        // Arrange
        var matrix = new double[,]
        {
            { 0.7, 0.3 },
            { 0.4, 0.6 }
        };

        // Act
        var result = _service.IsAbsorbingMatrix(matrix);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CalculateRegularChain_WithIdentityMatrix_ReturnsIdentity()
    {
        // Arrange
        var transitionMatrix = new double[,]
        {
            { 1.0, 0.0, 0.0 },
            { 0.0, 1.0, 0.0 },
            { 0.0, 0.0, 1.0 }
        };
        var initialState = new double[] { 0.5, 0.3, 0.2 };
        var data = new CalculationData(transitionMatrix, initialState);

        // Act
        var result = _service.CalculateRegularChain(data, 100);

        // Assert
        result.ResultVector.Should().BeEquivalentTo(initialState);
    }
}
