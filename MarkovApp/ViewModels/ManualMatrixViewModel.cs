using MarkovApp.Infrastructure;
using MarkovApp.Models;
using MarkovApp.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MarkovApp.ViewModels
{
    public class ManualMatrixViewModel : ObservableObject
    {
        private readonly IValidationService _validationService;
        private readonly IDialogService _dialogService;

        public ObservableCollection<CellViewModel> TransitionMatrix { get; }
        public ObservableCollection<InitialState> InitialStateVector { get; }

        public ICommand IncrementSizeCommand { get; }
        public ICommand SubmitCommand { get; }

        public event EventHandler<bool?>? RequestClose;

        public int Size { get; private set; }

        public ManualMatrixViewModel(
            IValidationService validationService,
            IDialogService dialogService)
        {
            _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            TransitionMatrix = new ObservableCollection<CellViewModel>();
            InitialStateVector = new ObservableCollection<InitialState>();

            IncrementSizeCommand = new RelayCommand(OnIncrementSize);
            SubmitCommand = new RelayCommand(OnSubmit);
        }

        private void OnIncrementSize(object? _) => IncrementSize();

        private void OnSubmit(object? _)
        {
            var data = GetCalculationData();
            if (data == null)
            {
                _dialogService.ShowError("Please enter valid numeric values in the transition matrix and initial state vector.");
                return;
            }
            if (!_validationService.ValidateCalculationData(data, out string? calcError))
            {
                _dialogService.ShowError(calcError!);
                return;
            }
            RequestClose?.Invoke(this, true);
        }

        private void IncrementSize()
        {
            int newSize = Size + 1;
            var oldCells = TransitionMatrix.ToList();
            TransitionMatrix.Clear();

            for (int row = 0; row < newSize; row++)
            {
                for (int col = 0; col < newSize; col++)
                {
                    var existing = oldCells.FirstOrDefault(c => c.Row == row && c.Column == col);
                    if (existing != null)
                        TransitionMatrix.Add(existing);
                    else
                        TransitionMatrix.Add(new CellViewModel(new Cell(row, col, 0)));
                }
            }

            var oldStates = InitialStateVector.ToList();
            InitialStateVector.Clear();

            for (int i = 0; i < newSize; i++)
            {
                if (i < oldStates.Count)
                    InitialStateVector.Add(oldStates[i]);
                else
                    InitialStateVector.Add(new InitialState(i, 0));
            }

            Size = newSize;
            OnPropertyChanged(nameof(Size));
        }

        public CalculationData? GetCalculationData()
        {
            if (Size <= 0) return null;

            var transitionMatrix = new double[Size, Size];
            var initialStatevector = new double[Size];

            foreach (var cellVM in TransitionMatrix)
            {
                if (!cellVM.TryGetValue(out double value)) return null;
                transitionMatrix[cellVM.Row, cellVM.Column] = value;
            }

            for (int i = 0; i < Size; i++)
            {
                if (!InitialStateVector[i].TryGetValue(out double value)) return null;
                initialStatevector[i] = value;
            }

            return new CalculationData(transitionMatrix, initialStatevector);
        }
    }
}