using MarkovApp.Infrastructure;
using MarkovApp.Models;
using MarkovApp.Services.Interfaces;
using MarkovApp.Utilities;
using System.Globalization;
using System.Windows.Input;

namespace MarkovApp.ViewModels
{
    public class EdgePropertiesViewModel : ObservableObject
    {
        private readonly IValidationService _validationService;
        private readonly IDialogService _dialogService;

        private string _edgeValueText;

        public Edge Edge { get; }

        public ICommand SubmitCommand { get; }

        public event EventHandler<bool?>? RequestClose;

        public EdgePropertiesViewModel(
            Edge edge,
            IValidationService validationService,
            IDialogService dialogService)
        {
            Edge = edge ?? throw new ArgumentNullException(nameof(edge));
            _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            _edgeValueText = edge.Value.ToString(CultureInfo.InvariantCulture);

            SubmitCommand = new RelayCommand(_ => OnSubmit());
        }

        public string EdgeValueText
        {
            get => _edgeValueText;
            set => SetProperty(ref _edgeValueText, value);
        }

        private void OnSubmit()
        {
            if (!TryParseValue(out double value))
                return;

            var tempEdge = new Edge
            {
                FromNode = Edge.FromNode,
                ToNode = Edge.ToNode,
                Value = value
            };

            if (!_validationService.ValidateEdge(tempEdge, out string? error))
            {
                _dialogService.ShowError(error!);
                return;
            }

            ApplyValue(value);
            RequestClose?.Invoke(this, true);
        }

        private bool TryParseValue(out double value)
        {
            bool isValid = NumericHelper.TryParseDoubleInvariant(EdgeValueText, out value);

            if (!isValid)
            {
                _dialogService.ShowError("Please enter a valid numeric value.");
                return false;
            }

            return true;
        }

        private void ApplyValue(double value)
        {
            Edge.Value = value;
        }
    }
}