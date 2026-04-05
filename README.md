# MarkovChainsCalculatorApp

<img width="2546" height="1385" alt="image" src="https://github.com/user-attachments/assets/d8e8b68f-8aef-46e1-9be0-c219f1574b40" />

A comprehensive Markov chain analysis solution featuring a WPF desktop application and a RESTful API, built with modern .NET architecture patterns, dependency injection, and comprehensive unit testing.

## Overview

**MarkovApp** is a multi-project solution for creating, visualizing, and analyzing Markov chains:

- **🖥️ MarkovApp (WPF Desktop)**: Interactive application for visualizing and analyzing Markov chains with drag-and-drop graph editing, built with MVVM architecture
- **🌐 MarkovApp.Api (REST API)**: RESTful API exposing the core calculation engine for programmatic access, enabling integration with external applications, cloud services, and automated workflows

The API extends the WPF application by providing HTTP-based access to the Markov chain calculation engine, making it suitable for microservices architectures, cloud deployments, and third-party integrations.

## Features

### WPF Desktop Application
- **Interactive Graph Editor**: Create and modify Markov chain graphs with a visual node-and-edge interface
- **Regular Markov Chain Analysis**: Calculate steady-state probabilities and long-term behavior
- **Absorbing Markov Chain Analysis**: Compute absorption probabilities and expected absorption times
- **Manual Matrix Input**: Enter transition matrices directly for analysis
- **State Persistence**: Save and load complete application state (JSON format)
- **Dark Green Theme**: Modern, visually appealing UI with consistent styling
- **Built-in Validation**: Real-time input validation for probabilities and matrix data

### REST API
- **RESTful Endpoints**: HTTP-based access to Markov chain calculations
- **OpenAPI Documentation**: Auto-generated API documentation with Swagger
- **Request Validation**: Comprehensive input validation with detailed error messages
- **Global Error Handling**: Consistent error responses with ProblemDetails
- **Structured Logging**: Request/response logging for observability
- **Cross-Platform**: Runs on Windows, Linux, and macOS

### Quality & Configuration
- **Configurable Settings**: appsettings.json with Options Pattern for easy customization
- **Comprehensive Testing**: 100 unit tests covering core business logic and API endpoints (~90% coverage)
- **Dependency Injection**: Modern .NET DI throughout the entire solution

## Technical Stack

### Core Technologies
- **Framework**: .NET 10.0 (Windows for WPF, cross-platform API)
- **Architecture**: MVVM (WPF) & RESTful API with Dependency Injection
- **Configuration**: Microsoft.Extensions.Configuration with Options Pattern
- **Testing**: xUnit with FluentAssertions and Moq
- **Documentation**: XML comments for API documentation and OpenAPI support

### WPF Application
- **UI Framework**: WPF (Windows Presentation Foundation)
- **Pattern**: MVVM with full Dependency Injection
- **Dependencies**:
  - `MathNet.Numerics` (v5.0.0) - Mathematical computations and matrix operations
  - `Microsoft.Xaml.Behaviors.Wpf` (v1.1.142) - XAML behavior support
  - `Microsoft.Extensions.Configuration.Json` (v10.0.5) - Configuration system
  - `Microsoft.Extensions.DependencyInjection` (v10.0.5) - DI container
  - `Microsoft.Extensions.Options.ConfigurationExtensions` (v10.0.5) - Options pattern

### REST API
- **Framework**: ASP.NET Core Minimal APIs
- **Documentation**: OpenAPI/Swagger with XML documentation
- **Error Handling**: Global exception handling with ProblemDetails
- **Logging**: Structured logging with Microsoft.Extensions.Logging
- **Dependencies**:
  - `Microsoft.AspNetCore.OpenApi` (v10.0.5) - OpenAPI specification generation

## Configuration

The application uses `appsettings.json` for configuration with strongly-typed settings classes:

```json
{
  "GraphSettings": {
    "MaxNodes": 12,
    "NodeRadius": 20.0,
    "MinNodeDistance": 50.0,
    "MinEdgeDistance": 50.0
  },
  "CalculationSettings": {
    "DefaultMaxIterations": 1000,
    "ConvergenceEpsilon": 0.0001
  }
}
```

You can modify these values without recompiling the application.

## Solution Structure

The solution consists of three projects:

### MarkovApp (Core WPF Application)
The desktop WPF application implementing the Markov chain visualizer and calculator with interactive graph editing capabilities.

### MarkovApp.Api (REST API)
RESTful API exposing the core Markov chain calculation engine for programmatic access. Includes comprehensive error handling, logging, and OpenAPI documentation.

### MarkovApp.Tests (Test Project)
Comprehensive unit tests covering both the WPF application and REST API using xUnit, FluentAssertions, and Moq.

## Project Structure

```
Solution 'MarkovApp'
│
├── MarkovApp/                      # Main Application Project
│   ├── App.xaml                    # Application entry point
│   ├── App.xaml.cs                 # Application startup & DI configuration
│   ├── MainWindow.xaml             # Main window UI definition
│   ├── MainWindow.xaml.cs          # Main window code-behind
│   ├── MarkovApp.csproj            # Project configuration file
│   │
│   ├── Infrastructure/             # Core MVVM framework
│   │   ├── DialogService.cs        # Dialog management service
│   │   ├── IDialogService.cs       # Dialog service interface
│   │   ├── ObservableObject.cs     # INotifyPropertyChanged base class
│   │   └── RelayCommand.cs         # ICommand implementation (generic & non-generic)
│   │
│   ├── Models/                     # Domain models
│   │   ├── AbsorbingMatrixResult.cs  # Absorbing chain calculation results
│   │   ├── AppState.cs             # Application state for persistence
│   │   ├── CalculationData.cs      # Input data for calculations
│   │   ├── Cell.cs                 # Transition matrix cell
│   │   ├── Edge.cs                 # Graph edge (transition)
│   │   ├── InitialState.cs         # Initial probability state
│   │   ├── Node.cs                 # Graph node (state)
│   │   └── RegularMatrixResult.cs  # Regular chain calculation results
│   │
│   ├── Services/                   # Business logic layer
│   │   ├── AppStateService.cs      # Save/load application state
│   │   ├── GraphDataService.cs     # Graph data transformations
│   │   ├── GraphLogicService.cs    # Graph manipulation logic
│   │   ├── MarkovCalculatorService.cs  # Core Markov chain calculations
│   │   ├── ValidationService.cs    # Input validation logic
│   │   └── Interfaces/             # Service interfaces
│   │       ├── IAppStateService.cs
│   │       ├── IGraphDataService.cs
│   │       ├── IGraphLogicService.cs
│   │       ├── IMarkovCalculatorService.cs
│   │       └── IValidationService.cs
│   │
│   ├── Utilities/                  # Helper utilities
│   │   ├── GraphGeometryHelper.cs  # Geometry calculations for rendering
│   │   └── NumericHelper.cs        # Numeric formatting & parsing
│   │
│   ├── ViewModels/                 # MVVM ViewModels
│   │   ├── AbsorbingMatrixResultViewModel.cs
│   │   ├── CellViewModel.cs        # Matrix cell editing
│   │   ├── EdgePropertiesViewModel.cs  # Edge property editor
│   │   ├── GraphViewModel.cs       # Main graph editor
│   │   ├── MainViewModel.cs        # Main application coordinator
│   │   ├── ManualMatrixViewModel.cs    # Manual matrix input
│   │   ├── MaxIterationsViewModel.cs   # Iteration configuration
│   │   ├── NodePropertiesViewModel.cs  # Node property editor
│   │   └── RegularMatrixResultViewModel.cs
│   │
│   ├── Views/                      # XAML views
│   │   ├── AbsorbingMatrixResultDisplay.xaml
│   │   ├── EdgeProperties.xaml
│   │   ├── ManualMatrixInput.xaml
│   │   ├── MaxIterationsInput.xaml
│   │   ├── NodeProperties.xaml
│   │   └── RegularMatrixResultDisplay.xaml
│   │
│   ├── Configuration/              # Configuration classes
│   │   ├── GraphSettings.cs        # Graph configuration settings
│   │   └── CalculationSettings.cs  # Calculation settings
│   │
│   ├── AppConfig.cs                # Static configuration accessor
│   ├── appsettings.json            # Application configuration
│   │
│   ├── Styles/                     # UI themes and styles
│       ├── DarkGreenTheme.xaml     # Color theme resources
│       └── DialogStyles.xaml       # Shared dialog styles
│
├── MarkovApp.Api/                  # REST API Project
│   ├── Program.cs                  # API startup & middleware configuration
│   ├── MarkovApp.Api.csproj        # API project configuration file
│   ├── appsettings.json            # API configuration
│   ├── MarkovApp.Api.http          # HTTP request examples
│   │
│   ├── Controllers/                # API Controllers
│   │   └── MarkovController.cs     # Markov chain calculation endpoints
│   │
│   └── DTOs/                       # Data Transfer Objects
│       ├── MarkovRequestDTO.cs     # Request model with validation
│       ├── RegularChainResponseDTO.cs
│       └── AbsorbingChainResponseDTO.cs
│
└── MarkovApp.Tests/                # Test Project (xUnit)
    ├── MarkovApp.Tests.csproj      # Test project configuration
    ├── TestHelper.cs               # Test configuration helper
    │
    ├── Infrastructure/             # Infrastructure tests (15 tests)
    │   ├── ObservableObjectTests.cs    # Property change notifications
    │   └── RelayCommandTests.cs        # Command execution & CanExecute
    │
    ├── Models/                     # Model tests (19 tests)
    │   ├── CalculationDataTests.cs
    │   ├── EdgeTests.cs
    │   └── NodeTests.cs
    │
    ├── Services/                   # Service tests (34 tests)
    │   ├── GraphDataServiceTests.cs        # Data transformation tests
    │   ├── GraphLogicServiceTests.cs       # Graph operation tests
    │   └── MarkovCalculatorServiceTests.cs # Calculation engine tests
    │
    ├── Utilities/                  # Utility tests (7 tests)
    │   └── GraphGeometryHelperTests.cs # Geometry calculation tests
    │
    ├── Controllers/                # API Controller tests (12 tests)
    │   └── MarkovControllerTests.cs    # API endpoint tests
    │
    └── DTOs/                       # DTO tests (13 tests)
        ├── MarkovRequestDtoTests.cs    # Request validation tests
        └── ResponseDtoTests.cs         # Response DTO tests
```

## Test Coverage

✅ **100 Unit Tests** covering:
- Markov chain calculations (regular & absorbing)
- Graph operations (nodes, edges, matrix updates)
- Data transformations and validation
- MVVM infrastructure (ObservableObject, RelayCommand)
- REST API endpoints and error handling
- DTO validation and serialization
- Logging and exception handling
- Error handling and edge cases

**Core business logic & API: ~90% coverage**

## Architecture

### MVVM Pattern with Dependency Injection

The application follows modern .NET practices with MVVM architecture and full dependency injection:

- **Models**: Represent domain entities (Node, Edge, calculation results)
- **Views**: XAML-based UI components with minimal code-behind
- **ViewModels**: Bridge between Models and Views, containing presentation logic and data binding
- **Services**: Business logic injected via constructor injection
- **Configuration**: Options Pattern with `IOptions<T>` for settings

### Key Components

#### 1. Configuration System
- **appsettings.json**: Centralized configuration file
- **GraphSettings**: Configurable graph parameters (max nodes, node radius, distances)
- **CalculationSettings**: Configurable calculation parameters (iterations, epsilon)
- **Options Pattern**: Strongly-typed configuration via `IOptions<T>`
- **AppConfig**: Static accessor for configuration in non-DI contexts

#### 2. Models
- **Node**: Represents a state in the Markov chain with configurable radius
- **Edge**: Represents a transition between states with a transition probability value
- **CalculationData**: Encapsulates all data needed for Markov chain calculations
- **RegularMatrixResult/AbsorbingMatrixResult**: Store calculation outcomes
- **AppState**: Serializable state for save/load functionality

#### 3. Services
- **MarkovCalculatorService**: Core calculation engine using MathNet.Numerics
  - Regular chain calculations with configurable convergence epsilon
  - Absorbing chain calculations with fundamental matrix computation
  - Matrix validation and comprehensive error handling
- **GraphLogicService**: Graph manipulation with configurable node limits
  - Node add/remove operations
  - Edge creation and deletion
  - Matrix synchronization with graph state
- **GraphDataService**: Data transformations between graph and calculation formats
- **ValidationService**: Centralized input validation
  - Probability validation for nodes and edges
  - Matrix and vector validation for calculations
  - App state validation (null check and node count limit)
- **AppStateService**: JSON-based state persistence with validation
  - Save/load application state to JSON files
  - Validates loaded state against maximum node configuration
- **DialogService**: Dialog window management

#### 4. ViewModels
- **MainViewModel**: Coordinates overall application functionality with DI
- **GraphViewModel**: Manages interactive graph editing (add/remove nodes and edges)
- **NodePropertiesViewModel/EdgePropertiesViewModel**: Property editing dialogs
- **Result ViewModels**: Display calculation results

#### 5. Infrastructure
- **ObservableObject**: Base class implementing INotifyPropertyChanged for data binding
- **RelayCommand/RelayCommand<T>**: ICommand implementations with primary constructors
- **DialogService**: Dialog abstraction for testability

#### 6. UI Styling
- **DarkGreenTheme.xaml**: Color palette and brush resources
- **DialogStyles.xaml**: Shared control styles for consistency
  - Dialog borders, buttons, textboxes
  - Result display styles
  - Consistent margins and spacing using Thickness resources

## Usage

### WPF Desktop Application

#### Creating a Markov Chain

1. **Add Nodes**: Click on the canvas to add states to your Markov chain
2. **Connect Nodes**: Create edges between nodes to define transitions
3. **Set Properties**:
   - Configure initial probabilities for each node
   - Set transition probabilities for edges
   - Mark absorbing states if applicable
4. **Calculate**: Choose between regular or absorbing chain analysis

#### Regular Markov Chain Analysis

For regular Markov chains (irreducible, aperiodic):
1. Click "Calculate Regular Chain"
2. Specify maximum iterations for convergence
3. View steady-state probabilities and convergence information

#### Absorbing Markov Chain Analysis

For chains with absorbing states:
1. Mark absorbing states in node properties
2. Click "Calculate Absorbing Chain"
3. View absorption probabilities and expected times

#### Manual Matrix Input

If you prefer to enter transition matrices directly:
1. Use "Manual Matrix Input" option
2. Define matrix dimensions
3. Enter transition probabilities
4. Perform calculations on the matrix

#### State Persistence

- **Save State**: Export current graph and configuration to a file
- **Load State**: Restore a previously saved state

### REST API

The MarkovApp.Api project exposes the core calculation engine as a RESTful API for programmatic access.

#### Running the API

```powershell
# Run the API locally
dotnet run --project MarkovApp.Api\MarkovApp.Api.csproj
```

The API will be available at `https://localhost:5264` (or the port specified in `launchSettings.json`).

#### OpenAPI/Swagger Documentation

When running in Development mode, navigate to `/openapi/v1.json` to view the OpenAPI specification.

#### API Endpoints

**POST /api/markov/calculate**

Calculate Markov chain properties for either regular or absorbing chains.

**Request Body (Regular Chain):**
```json
{
  "transitionMatrix": [
    [0.7, 0.3],
    [0.4, 0.6]
  ],
  "initialStateVector": [0.5, 0.5],
  "isAbsorbing": false,
  "maxIterations": 1000
}
```

**Response (Regular Chain):**
```json
{
  "steadyState": [0.571, 0.429],
  "resultVector": [0.55, 0.45]
}
```

**Request Body (Absorbing Chain):**
```json
{
  "transitionMatrix": [
    [1.0, 0.0, 0.0],
    [0.5, 0.0, 0.5],
    [0.0, 0.0, 1.0]
  ],
  "initialStateVector": [0.0, 1.0, 0.0],
  "isAbsorbing": true
}
```

**Response (Absorbing Chain):**
```json
{
  "fundamentalMatrix": [[1.0]],
  "absorptionProbabilities": [[0.5, 0.5]],
  "absorbingStates": [0, 2]
}
```

#### API Request Validation

The API validates:
- **TransitionMatrix**: Required, each row must sum to 1.0
- **InitialStateVector**: Required, must sum to 1.0
- **MaxIterations**: Optional, must be between 1 and 1,000,000 if specified

#### Example: Using curl

```bash
# Calculate regular chain
curl -X POST https://localhost:5264/api/markov/calculate \
  -H "Content-Type: application/json" \
  -d '{
    "transitionMatrix": [[0.7, 0.3], [0.4, 0.6]],
    "initialStateVector": [0.5, 0.5],
    "isAbsorbing": false,
    "maxIterations": 1000
  }'
```

#### Example: Using C#

```csharp
using System.Net.Http.Json;

var client = new HttpClient { BaseAddress = new Uri("https://localhost:5264") };

var request = new
{
    TransitionMatrix = new[] { new[] { 0.7, 0.3 }, new[] { 0.4, 0.6 } },
    InitialStateVector = new[] { 0.5, 0.5 },
    IsAbsorbing = false,
    MaxIterations = 1000
};

var response = await client.PostAsJsonAsync("/api/markov/calculate", request);
var result = await response.Content.ReadFromJsonAsync<RegularChainResponse>();
```

## Mathematical Background

### Regular Markov Chains
A Markov chain is regular if its transition matrix raised to some power has only positive entries. The application calculates:
- Steady-state probability vector π where πP = π
- Convergence over multiple iterations

### Absorbing Markov Chains
A state is absorbing if the probability of staying in that state is 1. The application computes:
- Fundamental matrix N = (I - Q)^-1
- Absorption probabilities using matrix B = NR
- Expected number of steps before absorption

## Development

### Prerequisites
- Visual Studio 2022 or later (with .NET 10.0 SDK)
- Windows 10/11

### Building the Solution
```powershell
dotnet build MarkovApp.slnx
```

### Building Individual Projects
```powershell
# WPF application
dotnet build MarkovApp\MarkovApp.csproj

# REST API
dotnet build MarkovApp.Api\MarkovApp.Api.csproj

# Test project
dotnet build MarkovApp.Tests\MarkovApp.Tests.csproj
```

### Running the Projects
```powershell
# Run WPF application
dotnet run --project MarkovApp\MarkovApp.csproj

# Run REST API
dotnet run --project MarkovApp.Api\MarkovApp.Api.csproj
```

### Running Tests
```powershell
# Run all tests
dotnet test MarkovApp.Tests\MarkovApp.Tests.csproj

# Run tests with detailed output
dotnet test MarkovApp.Tests\MarkovApp.Tests.csproj --logger "console;verbosity=normal"

# Run tests with code coverage
dotnet test MarkovApp.Tests\MarkovApp.Tests.csproj --collect:"XPlat Code Coverage"
```

### Formatting Code
```powershell
# Format entire solution
dotnet format MarkovApp.slnx
```

## Testing

The solution includes comprehensive unit tests covering core business logic:

### Test Technologies
- **xUnit** (v2.9.3) - Modern testing framework
- **FluentAssertions** (v8.9.0) - Readable assertion syntax
- **Moq** (v4.20.72) - Mocking framework for dependency injection
- **Microsoft.NET.Test.Sdk** (v17.14.1) - Test runner

### Test Configuration
Tests use a helper class (`TestHelper.cs`) to initialize configuration once for all tests, ensuring consistent behavior across test runs.

### Running Tests in Visual Studio
1. Open **Test Explorer**: `Test → Test Explorer` (Ctrl+E, T)
2. Click **Run All Tests**
3. View results and coverage

### Running Tests via CLI
```powershell
# Run all tests
dotnet test MarkovApp.Tests\MarkovApp.Tests.csproj

# Run with detailed output
dotnet test MarkovApp.Tests\MarkovApp.Tests.csproj --logger "console;verbosity=normal"

# Run with code coverage
dotnet test MarkovApp.Tests\MarkovApp.Tests.csproj --collect:"XPlat Code Coverage"
```

### Test Organization
Tests mirror the solution structure and cover all layers:

**WPF Application Tests (75 tests):**
- `Infrastructure/` - MVVM base class tests (15 tests)
- `Models/` - Domain model tests (19 tests)
- `Services/` - Business logic tests (34 tests)
- `Utilities/` - Helper class tests (7 tests)

**API Tests (25 tests):**
- `Controllers/` - API endpoint tests (12 tests)
- `DTOs/` - Request/response validation tests (13 tests)

**All 100 tests validate critical functionality including:**
- Markov chain mathematical correctness
- Graph state management with configurable parameters
- Data validation and error handling
- Configuration injection and usage
- REST API endpoints and error responses
- DTO validation and serialization
- Logging and exception handling
- Edge cases and boundary conditions

---

**Note**: The WPF application requires Windows to run due to its WPF framework dependency. The REST API is cross-platform and can run on Windows, Linux, or macOS.
