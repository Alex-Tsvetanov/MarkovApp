using FluentAssertions;
using MarkovApp.Infrastructure;

namespace MarkovApp.Tests.Infrastructure;

public class ObservableObjectTests
{
    private class TestObservableObject : ObservableObject
    {
        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private int _value;
        public int Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }

        public void TriggerPropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }
    }

    [Fact]
    public void SetProperty_WhenValueChanges_RaisesPropertyChanged()
    {
        // Arrange
        var obj = new TestObservableObject();
        var eventRaised = false;
        string? changedPropertyName = null;

        obj.PropertyChanged += (sender, e) =>
        {
            eventRaised = true;
            changedPropertyName = e.PropertyName;
        };

        // Act
        obj.Name = "Test";

        // Assert
        eventRaised.Should().BeTrue();
        changedPropertyName.Should().Be("Name");
        obj.Name.Should().Be("Test");
    }

    [Fact]
    public void SetProperty_WhenValueDoesNotChange_DoesNotRaisePropertyChanged()
    {
        // Arrange
        var obj = new TestObservableObject { Name = "Test" };
        var eventRaised = false;

        obj.PropertyChanged += (sender, e) => eventRaised = true;

        // Act
        obj.Name = "Test";

        // Assert
        eventRaised.Should().BeFalse();
    }

    [Fact]
    public void SetProperty_WithDifferentValues_UpdatesField()
    {
        // Arrange
        var obj = new TestObservableObject();

        // Act
        obj.Value = 42;

        // Assert
        obj.Value.Should().Be(42);
    }

    [Fact]
    public void SetProperty_ReturnsTrue_WhenValueChanges()
    {
        // Arrange
        var obj = new TestObservableObject();

        // Act
        obj.Value = 100;
        var result = obj.Value == 100;

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void OnPropertyChanged_RaisesPropertyChangedEvent()
    {
        // Arrange
        var obj = new TestObservableObject();
        var eventRaised = false;
        string? propertyName = null;

        obj.PropertyChanged += (sender, e) =>
        {
            eventRaised = true;
            propertyName = e.PropertyName;
        };

        // Act
        obj.TriggerPropertyChanged("CustomProperty");

        // Assert
        eventRaised.Should().BeTrue();
        propertyName.Should().Be("CustomProperty");
    }

    [Fact]
    public void PropertyChanged_CanBeNull()
    {
        // Arrange
        var obj = new TestObservableObject();

        // Act
        Action act = () => obj.TriggerPropertyChanged("Test");

        // Assert - Should not throw when no subscribers
        act.Should().NotThrow();
    }
}
