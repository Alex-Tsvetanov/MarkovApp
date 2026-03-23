using FluentAssertions;
using MarkovApp.Api.DTOs;

namespace MarkovApp.Tests.DTOs
{
    public class ResponseDtoTests
    {
        [Fact]
        public void RegularChainResponseDto_CanBeInstantiated()
        {
            // Arrange & Act
            var dto = new RegularChainResponseDto
            {
                SteadyState = new[] { 0.571, 0.429 },
                ResultVector = new[] { 0.55, 0.45 }
            };

            // Assert
            dto.SteadyState.Should().HaveCount(2);
            dto.ResultVector.Should().HaveCount(2);
            dto.SteadyState.Should().BeEquivalentTo(new[] { 0.571, 0.429 });
            dto.ResultVector.Should().BeEquivalentTo(new[] { 0.55, 0.45 });
        }

        [Fact]
        public void AbsorbingChainResponseDto_CanBeInstantiated()
        {
            // Arrange & Act
            var dto = new AbsorbingChainResponseDto
            {
                FundamentalMatrix = new double[,] { { 1.0, 0.5 }, { 0.5, 1.0 } },
                AbsorptionProbabilities = new double[,] { { 0.5, 0.5 } },
                AbsorbingStates = new List<int> { 0, 2 }
            };

            // Assert
            dto.FundamentalMatrix.Should().NotBeNull();
            dto.AbsorptionProbabilities.Should().NotBeNull();
            dto.AbsorbingStates.Should().HaveCount(2);
            dto.AbsorbingStates.Should().BeEquivalentTo(new[] { 0, 2 });
        }

        [Fact]
        public void AbsorbingChainResponseDto_DefaultAbsorbingStates_IsEmpty()
        {
            // Arrange & Act
            var dto = new AbsorbingChainResponseDto();

            // Assert
            dto.AbsorbingStates.Should().NotBeNull();
            dto.AbsorbingStates.Should().BeEmpty();
        }

        [Fact]
        public void RegularChainResponseDto_WithEmptyArrays_IsValid()
        {
            // Arrange & Act
            var dto = new RegularChainResponseDto
            {
                SteadyState = Array.Empty<double>(),
                ResultVector = Array.Empty<double>()
            };

            // Assert
            dto.SteadyState.Should().BeEmpty();
            dto.ResultVector.Should().BeEmpty();
        }

        [Fact]
        public void AbsorbingChainResponseDto_WithEmptyMatrices_IsValid()
        {
            // Arrange & Act
            var dto = new AbsorbingChainResponseDto
            {
                FundamentalMatrix = new double[0, 0],
                AbsorptionProbabilities = new double[0, 0],
                AbsorbingStates = new List<int>()
            };

            // Assert
            dto.FundamentalMatrix.GetLength(0).Should().Be(0);
            dto.AbsorptionProbabilities.GetLength(0).Should().Be(0);
            dto.AbsorbingStates.Should().BeEmpty();
        }
    }
}
