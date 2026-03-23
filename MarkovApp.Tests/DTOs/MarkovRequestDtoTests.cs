using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using MarkovApp.Api.DTOs;

namespace MarkovApp.Tests.DTOs
{
    public class MarkovRequestDtoTests
    {
        private static IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, validationContext, validationResults, true);
            return validationResults;
        }

        [Fact]
        public void MarkovRequestDto_WithValidData_PassesValidation()
        {
            // Arrange
            var dto = new MarkovRequestDto
            {
                TransitionMatrix = new[] { new[] { 0.7, 0.3 }, new[] { 0.4, 0.6 } },
                InitialStateVector = new[] { 0.5, 0.5 },
                IsAbsorbing = false,
                MaxIterations = 1000
            };

            // Act
            var validationResults = ValidateModel(dto);

            // Assert
            validationResults.Should().BeEmpty();
        }

        [Fact]
        public void MarkovRequestDto_WithNullTransitionMatrix_FailsValidation()
        {
            // Arrange
            var dto = new MarkovRequestDto
            {
                TransitionMatrix = null!,
                InitialStateVector = new[] { 0.5, 0.5 },
                IsAbsorbing = false
            };

            // Act
            var validationResults = ValidateModel(dto);

            // Assert
            validationResults.Should().ContainSingle();
            validationResults.First().ErrorMessage.Should().Contain("Transition matrix is required");
        }

        [Fact]
        public void MarkovRequestDto_WithNullInitialStateVector_FailsValidation()
        {
            // Arrange
            var dto = new MarkovRequestDto
            {
                TransitionMatrix = new[] { new[] { 0.7, 0.3 }, new[] { 0.4, 0.6 } },
                InitialStateVector = null!,
                IsAbsorbing = false
            };

            // Act
            var validationResults = ValidateModel(dto);

            // Assert
            validationResults.Should().ContainSingle();
            validationResults.First().ErrorMessage.Should().Contain("Initial state vector is required");
        }

        [Fact]
        public void MarkovRequestDto_WithMaxIterationsTooLow_FailsValidation()
        {
            // Arrange
            var dto = new MarkovRequestDto
            {
                TransitionMatrix = new[] { new[] { 0.7, 0.3 }, new[] { 0.4, 0.6 } },
                InitialStateVector = new[] { 0.5, 0.5 },
                IsAbsorbing = false,
                MaxIterations = 0
            };

            // Act
            var validationResults = ValidateModel(dto);

            // Assert
            validationResults.Should().ContainSingle();
            validationResults.First().ErrorMessage.Should().Contain("Max iterations must be between 1 and 1,000,000");
        }

        [Fact]
        public void MarkovRequestDto_WithMaxIterationsTooHigh_FailsValidation()
        {
            // Arrange
            var dto = new MarkovRequestDto
            {
                TransitionMatrix = new[] { new[] { 0.7, 0.3 }, new[] { 0.4, 0.6 } },
                InitialStateVector = new[] { 0.5, 0.5 },
                IsAbsorbing = false,
                MaxIterations = 2_000_000
            };

            // Act
            var validationResults = ValidateModel(dto);

            // Assert
            validationResults.Should().ContainSingle();
            validationResults.First().ErrorMessage.Should().Contain("Max iterations must be between 1 and 1,000,000");
        }

        [Fact]
        public void MarkovRequestDto_WithValidMaxIterationsAtBoundary_PassesValidation()
        {
            // Arrange
            var dto1 = new MarkovRequestDto
            {
                TransitionMatrix = new[] { new[] { 1.0 } },
                InitialStateVector = new[] { 1.0 },
                IsAbsorbing = false,
                MaxIterations = 1
            };

            var dto2 = new MarkovRequestDto
            {
                TransitionMatrix = new[] { new[] { 1.0 } },
                InitialStateVector = new[] { 1.0 },
                IsAbsorbing = false,
                MaxIterations = 1_000_000
            };

            // Act
            var results1 = ValidateModel(dto1);
            var results2 = ValidateModel(dto2);

            // Assert
            results1.Should().BeEmpty();
            results2.Should().BeEmpty();
        }

        [Fact]
        public void MarkovRequestDto_WithNullMaxIterations_PassesValidation()
        {
            // Arrange
            var dto = new MarkovRequestDto
            {
                TransitionMatrix = new[] { new[] { 0.7, 0.3 }, new[] { 0.4, 0.6 } },
                InitialStateVector = new[] { 0.5, 0.5 },
                IsAbsorbing = false,
                MaxIterations = null
            };

            // Act
            var validationResults = ValidateModel(dto);

            // Assert
            validationResults.Should().BeEmpty();
        }

        [Fact]
        public void MarkovRequestDto_AbsorbingChainWithoutMaxIterations_PassesValidation()
        {
            // Arrange
            var dto = new MarkovRequestDto
            {
                TransitionMatrix = new[] 
                { 
                    new[] { 1.0, 0.0, 0.0 }, 
                    new[] { 0.5, 0.0, 0.5 },
                    new[] { 0.0, 0.0, 1.0 }
                },
                InitialStateVector = new[] { 0.0, 1.0, 0.0 },
                IsAbsorbing = true
            };

            // Act
            var validationResults = ValidateModel(dto);

            // Assert
            validationResults.Should().BeEmpty();
        }
    }
}
