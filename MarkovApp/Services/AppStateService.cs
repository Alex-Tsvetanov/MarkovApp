using MarkovApp.Infrastructure;
using MarkovApp.Models;
using MarkovApp.Services.Interfaces;
using System.IO;
using System.Text.Json;

namespace MarkovApp.Services
{
    public class AppStateService : IAppStateService
    {
        private readonly IValidationService _validationService;
        private readonly IDialogService _dialogService;
        private readonly JsonSerializerOptions _jsonOptions;

        public AppStateService(IValidationService validationService, IDialogService dialogService)
        {
            _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };
        }

        public async Task SaveAsync(AppState state, string path)
        {
            ArgumentNullException.ThrowIfNull(state);

            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Path cannot be null or empty.", nameof(path));

            try
            {
                string json = JsonSerializer.Serialize(state, _jsonOptions);
                await File.WriteAllTextAsync(path, json);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to save app state to '{path}'.", ex);
            }
        }

        public async Task<AppState?> LoadAsync(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Path cannot be null or empty.", nameof(path));

            if (!File.Exists(path))
                return null;

            try
            {
                string json = await File.ReadAllTextAsync(path);
                AppState? state = JsonSerializer.Deserialize<AppState>(json, _jsonOptions);
                if (state != null)
                {
                    string? firstError = _validationService.ValidateAppState(state);
                    if (firstError != null)
                    {
                        _dialogService.ShowError(firstError);
                        return null;
                    }
                }

                return state;
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException("Failed to deserialize app state. File may be corrupted or invalid JSON.", ex);
            }
            catch (IOException ex)
            {
                throw new InvalidOperationException($"Failed to read file '{path}'.", ex);
            }
        }
    }
}