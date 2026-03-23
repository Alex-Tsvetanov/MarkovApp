using MarkovApp.Api.DTOs;
using MarkovApp.Models;
using MarkovApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MarkovApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class MarkovController : ControllerBase
    {
        private readonly IMarkovCalculatorService _calculatorService;
        private readonly IValidationService _validationService;
        private readonly ILogger<MarkovController> _logger;

        public MarkovController(
            IMarkovCalculatorService calculatorService,
            IValidationService validationService,
            ILogger<MarkovController> logger)
        {
            _calculatorService = calculatorService;
            _validationService = validationService;
            _logger = logger;
        }

        [HttpPost("calculate")]
        [ProducesResponseType(typeof(RegularChainResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AbsorbingChainResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Calculate([FromBody] MarkovRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state received");
                return BadRequest(ModelState);
            }

            try
            {
                var data = new CalculationData
                {
                    TransitionMatrix = ConvertTo2DArray(request.TransitionMatrix),
                    InitialStateVector = request.InitialStateVector
                };

                if (!_validationService.ValidateCalculationData(data, out var error))
                {
                    _logger.LogWarning("Validation failed: {Error}", error);
                    return BadRequest(new { error });
                }

                if (request.IsAbsorbing)
                {
                    _logger.LogInformation("Calculating absorbing chain");
                    var result = _calculatorService.CalculateAbsorbingChain(data);

                    return Ok(new AbsorbingChainResponseDto
                    {
                        FundamentalMatrix = result.AverageTransitions,
                        AbsorptionProbabilities = result.Probabilities,
                        AbsorbingStates = result.AbsorbingStates
                    });
                }
                else
                {
                    _logger.LogInformation("Calculating regular chain with max iterations: {MaxIterations}", request.MaxIterations ?? 1000);
                    var result = _calculatorService.CalculateRegularChain(
                        data,
                        request.MaxIterations
                    );

                    return Ok(new RegularChainResponseDto
                    {
                        SteadyState = result.LimitingProbabilities,
                        ResultVector = result.ResultVector
                    });
                }
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument in calculation");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during calculation");
                return BadRequest(new { error = ex.Message });
            }
        }

        private static double[,] ConvertTo2DArray(double[][] input)
        {
            int rows = input.Length;
            int cols = input[0].Length;
            var result = new double[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                if (input[i].Length != cols)
                    throw new ArgumentException("All rows must have the same length.");

                for (int j = 0; j < cols; j++)
                    result[i, j] = input[i][j];
            }

            return result;
        }
    }
}