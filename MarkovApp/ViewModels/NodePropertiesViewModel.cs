using MarkovApp.Infrastructure;
using MarkovApp.Models;
using MarkovApp.Services.Interfaces;
using MarkovApp.Utilities;
using System.Globalization;
using System.Windows.Input;

namespace MarkovApp.ViewModels
{
    public class NodePropertiesViewModel : ObservableObject
    {
        private readonly IValidationService _validationService;
        private readonly IDialogService _dialogService;

        private string _initialProbabilityText;
        private string _probabilityOfStayingText;

        public Node CurrentNode { get; }

        public ICommand SaveCommand { get; }

        public event EventHandler<bool?>? RequestClose;

        public NodePropertiesViewModel(Node node, IValidationService validationService,
    IDialogService dialogService)
        {
            CurrentNode = node ?? throw new ArgumentNullException(nameof(node));
            _validationService = validationService;
            _dialogService = dialogService;

            _initialProbabilityText = node.InitialProbability.ToString(CultureInfo.InvariantCulture);
            _probabilityOfStayingText = node.ProbabilityOfStaying.ToString(CultureInfo.InvariantCulture);

            SaveCommand = new RelayCommand(_ => OnSave());
        }

        public string InitialProbabilityText
        {
            get => _initialProbabilityText;
            set
            {
                SetProperty(ref _initialProbabilityText, value);
            }
        }

        public string ProbabilityOfStayingText
        {
            get => _probabilityOfStayingText;
            set
            {
                SetProperty(ref _probabilityOfStayingText, value);
            }
        }

        private void OnSave()
        {
            if (!TryParseValues(out double initial, out double staying))
                return;

            var tempNode = new Node
            {
                Id = CurrentNode.Id,
                InitialProbability = initial,
                ProbabilityOfStaying = staying
            };

            if (!_validationService.ValidateNode(tempNode, out string error))
            {
                _dialogService.ShowError(error);
                return;
            }

            ApplyValues(initial, staying);
            RequestClose?.Invoke(this, true);
        }

        private bool TryParseValues(out double initial, out double staying)
        {
            bool validInitial = NumericHelper.TryParseDoubleInvariant(InitialProbabilityText, out initial);
            bool validStaying = NumericHelper.TryParseDoubleInvariant(ProbabilityOfStayingText, out staying);

            if (!validInitial || !validStaying)
            {
                _dialogService.ShowError("Invalid Input.Please enter valid numeric values.");
                return false;
            }

            return true;
        }

        private void ApplyValues(double initial, double staying)
        {
            CurrentNode.InitialProbability = initial;
            CurrentNode.ProbabilityOfStaying = staying;
            CurrentNode.IsAbsorbing = staying == 1;
        }
    }
}