using System.Collections.Generic;
using System.Threading.Tasks;

namespace StoredProcedureAPI.Repositories
{
    public interface IStoredProcedureRepository
    {
        Task<IEnumerable<T>> ExecuteStoredProcedureAsync<T>(string procedureName, object parameters = null);
        Task<int> ExecuteNonQueryStoredProcedureAsync(string procedureName, object parameters = null);
        Task<IEnumerable<Models.StoredProcedureInfo>> GetAvailableStoredProceduresAsync();
        Task<bool> UpdateStoredProcedureVisibilityAsync(string procedureName, bool isPublic);
    }
}