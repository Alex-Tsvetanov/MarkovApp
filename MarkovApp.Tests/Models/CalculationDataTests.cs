using FluentAssertions;
using MarkovApp.Models;

namespace MarkovApp.Tests.Models;

public class CalculationDataTests
{
    [Fact]
    public void Constructor_WithValidData_InitializesCorrectly()
    {
        // Arrange
        var matrix = new double[,]
        {
            { 0.7, 0.3 },
            { 0.4, 0.6 }
        };
        var vector = new double[] { 1.0, 0.0 };

        // Act
        var data = new CalculationData(matrix, vector);

        // Assert
        data.TransitionMatrix.Should().BeEquivalentTo(matrix);
        data.InitialStateVector.Should().BeEquivalentTo(vector);
    }

    [Fact]
    public void Constructor_PreservesMatrixValues()
    {
        // Arrange
        var matrix = new double[,]
        {
            { 0.5, 0.25, 0.25 },
            { 0.3, 0.4, 0.3 },
            { 0.2, 0.2, 0.6 }
        };
        var vector = new double[] { 0.5, 0.3, 0.2 };

        // Act
        var data = new CalculationData(matrix, vector);

        // Assert
        data.TransitionMatrix[0, 0].Should().Be(0.5);
        data.TransitionMatrix[1, 2].Should().Be(0.3);
        data.TransitionMatrix[2, 2].Should().Be(0.6);
    }

    [Fact]
    public void Constructor_PreservesVectorValues()
    {
        // Arrange
        var matrix = new double[,] { { 1.0 } };
        var vector = new double[] { 0.75 };

        // Act
        var data = new CalculationData(matrix, vector);

        // Assert
        data.InitialStateVector[0].Should().Be(0.75);
    }

    [Fact]
    public void TransitionMatrix_CanBeAccessed()
    {
        // Arrange
        var matrix = new double[,]
        {
            { 0.8, 0.2 },
            { 0.3, 0.7 }
        };
        var vector = new double[] { 0.6, 0.4 };
        var data = new CalculationData(matrix, vector);

        // Act
        var retrievedMatrix = data.TransitionMatrix;

        // Assert
        retrievedMatrix.Should().NotBeNull();
        retrievedMatrix.GetLength(0).Should().Be(2);
        retrievedMatrix.GetLength(1).Should().Be(2);
    }

    [Fact]
    public void InitialStateVector_CanBeAccessed()
    {
        // Arrange
        var matrix = new double[,] { { 1.0 } };
        var vector = new double[] { 1.0 };
        var data = new CalculationData(matrix, vector);

        // Act
        var retrievedVector = data.InitialStateVector;

        // Assert
        retrievedVector.Should().NotBeNull();
        retrievedVector.Length.Should().Be(1);
        retrievedVector[0].Should().Be(1.0);
    }
}
