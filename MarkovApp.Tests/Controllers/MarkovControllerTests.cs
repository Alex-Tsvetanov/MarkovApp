using FluentAssertions;
using MarkovApp.Api.Controllers;
using MarkovApp.Api.DTOs;
using MarkovApp.Models;
using MarkovApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace MarkovApp.Tests.Controllers
{
    public class MarkovControllerTests
    {
        private readonly Mock<IMarkovCalculatorService> _mockCalculatorService;
        private readonly Mock<IValidationService> _mockValidationService;
        private readonly Mock<ILogger<MarkovController>> _mockLogger;
        private readonly MarkovController _controller;

        public MarkovControllerTests()
        {
            _mockCalculatorService = new Mock<IMarkovCalculatorService>();
            _mockValidationService = new Mock<IValidationService>();
            _mockLogger = new Mock<ILogger<MarkovController>>();
            _controller = new MarkovController(
                _mockCalculatorService.Object,
                _mockValidationService.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public void Calculate_WithValidRegularChain_ReturnsOkWithCorrectResponse()
        {
            // Arrange
            var request = new MarkovRequestDto
            {
                TransitionMatrix = new[] { new[] { 0.7, 0.3 }, new[] { 0.4, 0.6 } },
                InitialStateVector = new[] { 0.5, 0.5 },
                IsAbsorbing = false,
                MaxIterations = 1000
            };

            var expectedResult = new RegularMatrixResult(
                new double[,] { { 0.7, 0.3 }, { 0.4, 0.6 } },
                new[] { 0.571, 0.429 },
                new[] { 0.55, 0.45 }
            );

            _mockValidationService
                .Setup(v => v.ValidateCalculationData(It.IsAny<CalculationData>(), out It.Ref<string?>.IsAny))
                .Returns(true);

            _mockCalculatorService
                .Setup(c => c.CalculateRegularChain(It.IsAny<CalculationData>(), It.IsAny<int?>()))
                .Returns(expectedResult);

            // Act
            var result = _controller.Calculate(request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeOfType<RegularChainResponseDto>();

            var response = okResult.Value as RegularChainResponseDto;
            response!.SteadyState.Should().BeEquivalentTo(expectedResult.LimitingProbabilities);
            response.ResultVector.Should().BeEquivalentTo(expectedResult.ResultVector);
        }

        [Fact]
        public void Calculate_WithValidAbsorbingChain_ReturnsOkWithCorrectResponse()
        {
            // Arrange
            var request = new MarkovRequestDto
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

            var expectedResult = new AbsorbingMatrixResult(
                new double[,] { { 1.0 } },
                new double[,] { { 0.5, 0.5 } },
                new List<int> { 0, 2 }
            );

            _mockValidationService
                .Setup(v => v.ValidateCalculationData(It.IsAny<CalculationData>(), out It.Ref<string?>.IsAny))
                .Returns(true);

            _mockCalculatorService
                .Setup(c => c.CalculateAbsorbingChain(It.IsAny<CalculationData>()))
                .Returns(expectedResult);

            // Act
            var result = _controller.Calculate(request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeOfType<AbsorbingChainResponseDto>();

            var response = okResult.Value as AbsorbingChainResponseDto;
            response!.FundamentalMatrix.Should().BeEquivalentTo(expectedResult.AverageTransitions);
            response.AbsorptionProbabilities.Should().BeEquivalentTo(expectedResult.Probabilities);
            response.AbsorbingStates.Should().BeEquivalentTo(expectedResult.AbsorbingStates);
        }

        [Fact]
        public void Calculate_WithInvalidTransitionMatrix_ReturnsBadRequest()
        {
            // Arrange
            var request = new MarkovRequestDto
            {
                TransitionMatrix = new[] { new[] { 0.7, 0.3 }, new[] { 0.4, 0.5 } }, // Doesn't sum to 1
                InitialStateVector = new[] { 0.5, 0.5 },
                IsAbsorbing = false
            };

            string? errorMessage = "Row 2 of the transition matrix does not sum to 1.";
            _mockValidationService
                .Setup(v => v.ValidateCalculationData(It.IsAny<CalculationData>(), out errorMessage))
                .Returns(false);

            // Act
            var result = _controller.Calculate(request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult!.Value.Should().NotBeNull();
        }

        [Fact]
        public void Calculate_WithNullTransitionMatrix_ReturnsBadRequest()
        {
            // Arrange
            var request = new MarkovRequestDto
            {
                TransitionMatrix = null!,
                InitialStateVector = new[] { 0.5, 0.5 },
                IsAbsorbing = false
            };

            // Act
            _controller.ModelState.AddModelError("TransitionMatrix", "Transition matrix is required");
            var result = _controller.Calculate(request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void Calculate_WithInvalidInitialStateVector_ReturnsBadRequest()
        {
            // Arrange
            var request = new MarkovRequestDto
            {
                TransitionMatrix = new[] { new[] { 0.7, 0.3 }, new[] { 0.4, 0.6 } },
                InitialStateVector = new[] { 0.5, 0.6 }, // Doesn't sum to 1
                IsAbsorbing = false
            };

            string? errorMessage = "Initial state vector does not sum to 1.";
            _mockValidationService
                .Setup(v => v.ValidateCalculationData(It.IsAny<CalculationData>(), out errorMessage))
                .Returns(false);

            // Act
            var result = _controller.Calculate(request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void Calculate_WithJaggedTransitionMatrix_ReturnsBadRequest()
        {
            // Arrange
            var request = new MarkovRequestDto
            {
                TransitionMatrix = new[] { new[] { 0.7, 0.3 }, new[] { 1.0 } }, // Different row lengths
                InitialStateVector = new[] { 0.5, 0.5 },
                IsAbsorbing = false
            };

            // Act
            var result = _controller.Calculate(request);

            // Assert
            // The controller should catch the ArgumentException from ConvertTo2DArray and return BadRequest
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            var value = badRequestResult!.Value;

            var errorProperty = value!.GetType().GetProperty("error");
            errorProperty.Should().NotBeNull();
            var errorMessage = errorProperty!.GetValue(value) as string;
            errorMessage.Should().Contain("All rows must have the same length");
        }

        [Fact]
        public void Calculate_WhenCalculatorThrowsException_ReturnsBadRequest()
        {
            // Arrange
            var request = new MarkovRequestDto
            {
                TransitionMatrix = new[] { new[] { 0.7, 0.3 }, new[] { 0.4, 0.6 } },
                InitialStateVector = new[] { 0.5, 0.5 },
                IsAbsorbing = false
            };

            _mockValidationService
                .Setup(v => v.ValidateCalculationData(It.IsAny<CalculationData>(), out It.Ref<string?>.IsAny))
                .Returns(true);

            _mockCalculatorService
                .Setup(c => c.CalculateRegularChain(It.IsAny<CalculationData>(), It.IsAny<int?>()))
                .Throws(new InvalidOperationException("Calculation failed"));

            // Act
            var result = _controller.Calculate(request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            var value = badRequestResult!.Value;

            var errorProperty = value!.GetType().GetProperty("error");
            errorProperty.Should().NotBeNull();
            var errorMessage = errorProperty!.GetValue(value) as string;
            errorMessage.Should().Contain("Calculation failed");
        }

        [Fact]
        public void Calculate_WithMaxIterations_PassesValueToService()
        {
            // Arrange
            var request = new MarkovRequestDto
            {
                TransitionMatrix = new[] { new[] { 0.7, 0.3 }, new[] { 0.4, 0.6 } },
                InitialStateVector = new[] { 0.5, 0.5 },
                IsAbsorbing = false,
                MaxIterations = 5000
            };

            var expectedResult = new RegularMatrixResult(
                new double[,] { { 0.7, 0.3 }, { 0.4, 0.6 } },
                new[] { 0.571, 0.429 },
                new[] { 0.55, 0.45 }
            );

            _mockValidationService
                .Setup(v => v.ValidateCalculationData(It.IsAny<CalculationData>(), out It.Ref<string?>.IsAny))
                .Returns(true);

            _mockCalculatorService
                .Setup(c => c.CalculateRegularChain(It.IsAny<CalculationData>(), 5000))
                .Returns(expectedResult);

            // Act
            var result = _controller.Calculate(request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            _mockCalculatorService.Verify(
                c => c.CalculateRegularChain(It.IsAny<CalculationData>(), 5000),
                Times.Once
            );
        }

        [Fact]
        public void Calculate_WithArgumentException_ReturnsBadRequestWithMessage()
        {
            // Arrange
            var request = new MarkovRequestDto
            {
                TransitionMatrix = new[] { new[] { 0.7, 0.3 }, new[] { 0.4, 0.6 } },
                InitialStateVector = new[] { 0.5, 0.5 },
                IsAbsorbing = false
            };

            _mockValidationService
                .Setup(v => v.ValidateCalculationData(It.IsAny<CalculationData>(), out It.Ref<string?>.IsAny))
                .Returns(true);

            _mockCalculatorService
                .Setup(c => c.CalculateRegularChain(It.IsAny<CalculationData>(), It.IsAny<int?>()))
                .Throws(new ArgumentException("Invalid matrix dimensions"));

            // Act
            var result = _controller.Calculate(request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            var value = badRequestResult!.Value;

            var errorProperty = value!.GetType().GetProperty("error");
            errorProperty.Should().NotBeNull();
            var errorMessage = errorProperty!.GetValue(value) as string;
            errorMessage.Should().Be("Invalid matrix dimensions");
        }

        [Fact]
        public void Calculate_VerifiesLoggingOccurs()
        {
            // Arrange
            var request = new MarkovRequestDto
            {
                TransitionMatrix = new[] { new[] { 0.7, 0.3 }, new[] { 0.4, 0.6 } },
                InitialStateVector = new[] { 0.5, 0.5 },
                IsAbsorbing = false,
                MaxIterations = 1000
            };

            var expectedResult = new RegularMatrixResult(
                new double[,] { { 0.7, 0.3 }, { 0.4, 0.6 } },
                new[] { 0.571, 0.429 },
                new[] { 0.55, 0.45 }
            );

            _mockValidationService
                .Setup(v => v.ValidateCalculationData(It.IsAny<CalculationData>(), out It.Ref<string?>.IsAny))
                .Returns(true);

            _mockCalculatorService
                .Setup(c => c.CalculateRegularChain(It.IsAny<CalculationData>(), It.IsAny<int?>()))
                .Returns(expectedResult);

            // Act
            var result = _controller.Calculate(request);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Calculating regular chain")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public void Calculate_LogsWarningWhenValidationFails()
        {
            // Arrange
            var request = new MarkovRequestDto
            {
                TransitionMatrix = new[] { new[] { 0.7, 0.3 }, new[] { 0.4, 0.5 } },
                InitialStateVector = new[] { 0.5, 0.5 },
                IsAbsorbing = false
            };

            string? errorMessage = "Row 2 of the transition matrix does not sum to 1.";
            _mockValidationService
                .Setup(v => v.ValidateCalculationData(It.IsAny<CalculationData>(), out errorMessage))
                .Returns(false);

            // Act
            var result = _controller.Calculate(request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Validation failed")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public void Calculate_WithAbsorbingChain_LogsAppropriateMessage()
        {
            // Arrange
            var request = new MarkovRequestDto
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

            var expectedResult = new AbsorbingMatrixResult(
                new double[,] { { 1.0 } },
                new double[,] { { 0.5, 0.5 } },
                new List<int> { 0, 2 }
            );

            _mockValidationService
                .Setup(v => v.ValidateCalculationData(It.IsAny<CalculationData>(), out It.Ref<string?>.IsAny))
                .Returns(true);

            _mockCalculatorService
                .Setup(c => c.CalculateAbsorbingChain(It.IsAny<CalculationData>()))
                .Returns(expectedResult);

            // Act
            var result = _controller.Calculate(request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Calculating absorbing chain")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}
