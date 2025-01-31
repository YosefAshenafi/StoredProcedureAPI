using Microsoft.AspNetCore.Mvc;
using StoredProcedureAPI.Services;

namespace StoredProcedureAPI.Controllers
{
    public class StoredProcedureController : Controller
    {
        private readonly ILogger<StoredProcedureController> _logger;
        private readonly IStoredProcedureService _storedProcedureService;

        public StoredProcedureController(
            ILogger<StoredProcedureController> logger,
            IStoredProcedureService storedProcedureService)
        {
            _logger = logger;
            _storedProcedureService = storedProcedureService;
        }

        public async Task<IActionResult> Index()
        {
            var procedures = await _storedProcedureService.GetAllStoredProceduresAsync();
            return View(procedures);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateVisibility(string procedureName, bool isPublic)
        {
            _logger.LogInformation("UpdateVisibility called with procedureName: {ProcedureName}, isPublic: {IsPublic}", 
        procedureName, isPublic);

            await _storedProcedureService.UpdateStoredProcedureVisibilityAsync(procedureName, isPublic);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(string name)
        {
            var procedure = await _storedProcedureService.GetStoredProcedureDetailsAsync(name);
            if (procedure == null)
                return NotFound();
            
            return View(procedure);
        }
    }
}