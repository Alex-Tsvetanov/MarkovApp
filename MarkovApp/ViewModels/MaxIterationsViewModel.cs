using MarkovApp.Infrastructure;
using MarkovApp.Services.Interfaces;
using System.Windows.Input;

namespace MarkovApp.ViewModels
{
    public class MaxIterationsViewModel : ObservableObject
    {
        private readonly IValidationService _validationService;
        private readonly IDialogService _dialogService;

        private string _maxIterationsText;

        public ICommand SubmitCommand { get; }

        public event EventHandler<bool?>? RequestClose;

        public MaxIterationsViewModel(IValidationService validationService,
            IDialogService dialogService)
        {
            _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            _maxIterationsText = string.Empty;

            SubmitCommand = new RelayCommand(_ => OnSubmit());
        }

        public string MaxIterationsText
        {
            get => _maxIterationsText;
            set => SetProperty(ref _maxIterationsText, value);
        }

        public int? MaxIterations { get; private set; }

        private void OnSubmit()
        {
            if (!TryParseValue(out int value))
                return;

            if (!_validationService.ValidateMaxIterations(value, out string error))
            {
                _dialogService.ShowError(error);
                return;
            }

            MaxIterations = value;
            RequestClose?.Invoke(this, true);
        }

        private bool TryParseValue(out int value)
        {
            if (!int.TryParse(MaxIterationsText, out value))
            {
                _dialogService.ShowError("Please enter a valid integer for maximum iterations.");
                return false;
            }

            return true;
        }
    }
}