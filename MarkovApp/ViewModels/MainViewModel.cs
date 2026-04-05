using MarkovApp.Infrastructure;
using MarkovApp.Services;
using MarkovApp.Services.Interfaces;
using System.Windows.Input;

namespace MarkovApp.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private readonly IMarkovCalculatorService _calculatorService;
        private readonly IDialogService _dialogService;
        private readonly IAppStateService _appStateService;
        private readonly IValidationService _validationService;
        private readonly LanguageService _languageService;
        private bool _isBusy = false;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (SetProperty(ref _isBusy, value))
                {
                    ((RelayCommand)CalculateRegularCommand).RaiseCanExecuteChanged();
                    ((RelayCommand)CalculateAbsorbingCommand).RaiseCanExecuteChanged();
                    ((RelayCommand)SaveFullStateCommand).RaiseCanExecuteChanged();
                    ((RelayCommand)LoadFullStateCommand).RaiseCanExecuteChanged();
                }
            }
        }
        public GraphViewModel GraphViewModel { get; }

        public ICommand CalculateRegularCommand { get; }
        public ICommand CalculateAbsorbingCommand { get; }
        public ICommand OpenManualMatrixInputCommand { get; }
        public ICommand SaveFullStateCommand { get; }
        public ICommand LoadFullStateCommand { get; }

        public ICommand ClearGraphCommand { get; }
        public ICommand ToggleLanguageCommand { get; }

        public MainViewModel(GraphViewModel graphViewModel,
                             IMarkovCalculatorService calculatorService,
                             IDialogService dialogService,
                             IAppStateService appStateService,
                             IValidationService validationService,
                             LanguageService languageService)
        {
            GraphViewModel = graphViewModel;
            _calculatorService = calculatorService;
            _dialogService = dialogService;
            _appStateService = appStateService;
            _validationService = validationService;
            _languageService = languageService;

            CalculateRegularCommand = new RelayCommand(async _ => await CalculateRegularAsync(), _ => !IsBusy);
            CalculateAbsorbingCommand = new RelayCommand(async _ => await CalculateAbsorbingAsync(), _ => !IsBusy);
            OpenManualMatrixInputCommand = new RelayCommand(_ => OpenManualMatrixInput());
            SaveFullStateCommand = new RelayCommand(async _ => await SaveStateAsync(), _ => !IsBusy);
            LoadFullStateCommand = new RelayCommand(async _ => await LoadStateAsync(), _ => !IsBusy);
            ClearGraphCommand = new RelayCommand(_ => ClearGraph());
            ToggleLanguageCommand = new RelayCommand(_ => _languageService.Toggle());
        }
        private async Task CalculateRegularAsync()
        {
            var data = GraphViewModel.ToCalculationData();
            if (!_validationService.ValidateCalculationData(data, out string? error))
            {
                _dialogService.ShowError(error!);
                return;
            }

            var maxIterationsVM = new MaxIterationsViewModel(_validationService, _dialogService);
            int? maxIterations = _dialogService.ShowMaxIterationsInput(maxIterationsVM);

            if (maxIterations == null) return;

            try
            {
                IsBusy = true;
                data = GraphViewModel.ToCalculationData();
                var result = await Task.Run(() => _calculatorService.CalculateRegularChain(data, maxIterations));

                var regularResultVM = new RegularMatrixResultViewModel(result);
                _dialogService.ShowRegularMatrixResult(regularResultVM);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task CalculateAbsorbingAsync()
        {
            var data = GraphViewModel.ToCalculationData();
            if (!_validationService.ValidateCalculationData(data, out string? error))
            {
                _dialogService.ShowError(error!);
                return;
            }
            try
            {
                IsBusy = true;
                data = GraphViewModel.ToCalculationData();
                var result = await Task.Run(() => _calculatorService.CalculateAbsorbingChain(data));

                var absorbingResultVM = new AbsorbingMatrixResultViewModel(result);
                _dialogService.ShowAbsorbingMatrixResult(absorbingResultVM);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void OpenManualMatrixInput()
        {
            var manualVM = new ManualMatrixViewModel(_validationService, _dialogService);
            var updatedData = _dialogService.ShowManualMatrixInput(manualVM);

            if (updatedData != null)
            {
                GraphViewModel.SyncCalculationData(updatedData);
                GraphViewModel.SetManualMode();
            }
        }
        private async Task SaveStateAsync()
        {
            var state = GraphViewModel.ToAppState(true);
            var file = _dialogService.ShowSaveFileDialog();
            if (file == null) return;

            IsBusy = true;
            try
            {
                await _appStateService.SaveAsync(state, file);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoadStateAsync()
        {
            var file = _dialogService.ShowLoadFileDialog();
            if (file == null) return;

            IsBusy = true;
            try
            {
                var state = await _appStateService.LoadAsync(file);
                if (state != null)
                    GraphViewModel.LoadFromAppState(state);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void ClearGraph()
        {
            if (!_dialogService.ShowConfirmation(
                "Clear All Data",
                "Are you sure you want to clear all nodes, edges, transition matrix, and initial state vector? This cannot be undone."))
                return;

            GraphViewModel.Nodes.Clear();
            GraphViewModel.Edges.Clear();
            GraphViewModel.TransitionMatrix.Clear();
            GraphViewModel.InitialStateVector.Clear();
            GraphViewModel.ResetOverrides();
            GraphViewModel.ResetNodeCounter();
        }
    }
}