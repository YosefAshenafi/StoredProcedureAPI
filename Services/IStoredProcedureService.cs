using StoredProcedureAPI.Models;

namespace StoredProcedureAPI.Services
{
    public interface IStoredProcedureService
    {
        Task<IEnumerable<StoredProcedureViewModel>> GetAllStoredProceduresAsync();
        Task<StoredProcedureViewModel> GetStoredProcedureDetailsAsync(string name);
        Task UpdateStoredProcedureVisibilityAsync(string name, bool isVisible);
        bool GetProcedureVisibility(string procedureName);
    }
}