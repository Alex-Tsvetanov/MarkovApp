using MarkovApp.Models;

namespace MarkovApp.Services.Interfaces
{
    public interface IAppStateService
    {
        Task SaveAsync(AppState state, string path);
        Task<AppState?> LoadAsync(string path);
    }
}