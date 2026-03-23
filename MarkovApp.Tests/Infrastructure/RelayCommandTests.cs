using FluentAssertions;
using MarkovApp.Infrastructure;

namespace MarkovApp.Tests.Infrastructure;

public class RelayCommandTests
{
    [Fact]
    public void Execute_CallsProvidedAction()
    {
        // Arrange
        bool executed = false;
        var command = new RelayCommand(_ => executed = true);

        // Act
        command.Execute(null);

        // Assert
        executed.Should().BeTrue();
    }

    [Fact]
    public void Execute_PassesParameterToAction()
    {
        // Arrange
        object? receivedParameter = null;
        var command = new RelayCommand(param => receivedParameter = param);
        var testParam = "test parameter";

        // Act
        command.Execute(testParam);

        // Assert
        receivedParameter.Should().Be(testParam);
    }

    [Fact]
    public void CanExecute_WithoutPredicate_ReturnsTrue()
    {
        // Arrange
        var command = new RelayCommand(_ => { });

        // Act
        var result = command.CanExecute(null);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanExecute_WithPredicate_ReturnsPredicateResult()
    {
        // Arrange
        var command = new RelayCommand(_ => { }, _ => false);

        // Act
        var result = command.CanExecute(null);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanExecute_WithPredicate_PassesParameter()
    {
        // Arrange
        object? receivedParameter = null;
        var command = new RelayCommand(
            _ => { },
            param =>
            {
                receivedParameter = param;
                return true;
            });
        var testParam = "test";

        // Act
        command.CanExecute(testParam);

        // Assert
        receivedParameter.Should().Be(testParam);
    }

    [Fact]
    public void RaiseCanExecuteChanged_RaisesEvent()
    {
        // Arrange
        var command = new RelayCommand(_ => { });
        var eventRaised = false;
        command.CanExecuteChanged += (_, _) => eventRaised = true;

        // Act
        command.RaiseCanExecuteChanged();

        // Assert
        eventRaised.Should().BeTrue();
    }

    [Fact]
    public void Constructor_WithNullExecute_ThrowsArgumentNullException()
    {
        // Act
        Action act = () => new RelayCommand(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Execute_WithComplexAction_ExecutesSuccessfully()
    {
        // Arrange
        int counter = 0;
        var command = new RelayCommand(_ => counter += 10);

        // Act
        command.Execute(null);
        command.Execute(null);

        // Assert
        counter.Should().Be(20);
    }
}

public class RelayCommandGenericTests
{
    [Fact]
    public void Execute_CallsProvidedAction_WithTypedParameter()
    {
        // Arrange
        string? receivedValue = null;
        var command = new RelayCommand<string>(value => receivedValue = value);

        // Act
        command.Execute("test");

        // Assert
        receivedValue.Should().Be("test");
    }

    [Fact]
    public void CanExecute_WithPredicate_ReturnsPredicateResult()
    {
        // Arrange
        var command = new RelayCommand<int>(_ => { }, value => value > 10);

        // Act
        var result1 = command.CanExecute(15);
        var result2 = command.CanExecute(5);

        // Assert
        result1.Should().BeTrue();
        result2.Should().BeFalse();
    }

    [Fact]
    public void CanExecute_WithoutPredicate_ReturnsTrue()
    {
        // Arrange
        var command = new RelayCommand<string>(_ => { });

        // Act
        var result = command.CanExecute("anything");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void RaiseCanExecuteChanged_RaisesEvent()
    {
        // Arrange
        var command = new RelayCommand<int>(_ => { });
        var eventRaised = false;
        command.CanExecuteChanged += (_, _) => eventRaised = true;

        // Act
        command.RaiseCanExecuteChanged();

        // Assert
        eventRaised.Should().BeTrue();
    }

    [Fact]
    public void Constructor_WithNullExecute_ThrowsArgumentNullException()
    {
        // Act
        Action act = () => new RelayCommand<string>(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Execute_WithComplexType_ExecutesSuccessfully()
    {
        // Arrange
        var receivedList = new List<int>();
        var command = new RelayCommand<List<int>>(list => receivedList.AddRange(list));
        var testList = new List<int> { 1, 2, 3 };

        // Act
        command.Execute(testList);

        // Assert
        receivedList.Should().BeEquivalentTo(testList);
    }
}
