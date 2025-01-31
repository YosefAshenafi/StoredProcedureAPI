using StoredProcedureAPI.Models;
using StoredProcedureAPI.Repositories;

namespace StoredProcedureAPI.Services
{
    public class StoredProcedureService : IStoredProcedureService
    {
        private readonly IStoredProcedureRepository _repository;

        public StoredProcedureService(IStoredProcedureRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<StoredProcedureViewModel>> GetAllStoredProceduresAsync()
        {
            var procedures = await _repository.GetAvailableStoredProceduresAsync();
            return procedures.Select(p => new StoredProcedureViewModel
            {
                Name = p.Name,
                IsVisible = p.IsPublic,
            });
        }

        public async Task<StoredProcedureViewModel> GetStoredProcedureDetailsAsync(string name)
        {
            var procedures = await _repository.GetAvailableStoredProceduresAsync();
            var procedure = procedures.FirstOrDefault(p => p.Name == name);
            
            if (procedure == null)
                return null;

            return new StoredProcedureViewModel
            {
                Name = procedure.Name,
                IsVisible = procedure.IsPublic
            };
        }

        public async Task UpdateStoredProcedureVisibilityAsync(string procedure_name, bool isPublic)
        {
            await _repository.UpdateStoredProcedureVisibilityAsync(procedure_name, isPublic);
        }

        public bool GetProcedureVisibility(string procedureName)
        {
            var procedures = _repository.GetAvailableStoredProceduresAsync().Result;
            var procedure = procedures.FirstOrDefault(p => p.Name == procedureName);
            return procedure?.IsPublic ?? false;
        }
    }
}