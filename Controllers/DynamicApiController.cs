using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using StoredProcedureAPI.Repositories;
using System.Text.Json;
using StoredProcedureAPI.Models;

[ApiController]
[Route("api/[controller]")]
public class DynamicApiController : ControllerBase
{
    private readonly IStoredProcedureRepository _repository;
    private static readonly ConcurrentDictionary<string, bool> _procedureCache = new();

    public DynamicApiController(IStoredProcedureRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("procedures")]
    public async Task<IActionResult> GetAvailableProcedures()
    {
        var procedures = await _repository.GetAvailableStoredProceduresAsync();
        var publicProcedures = procedures.Where(p => p.IsPublic);
        return Ok(publicProcedures);
    }

    [HttpPost("procedures/{procedureName}/visibility")]
    public async Task<IActionResult> UpdateProcedureVisibility(string procedureName, [FromBody] bool isPublic)
    {
        var procedures = await _repository.GetAvailableStoredProceduresAsync();
        var procInfo = procedures.FirstOrDefault(p => p.Name.Equals(procedureName, StringComparison.OrdinalIgnoreCase));
        
        if (procInfo == null || !procInfo.IsPublic)
            return Ok(new { });  // Return empty object instead of error

        var result = await _repository.UpdateStoredProcedureVisibilityAsync(procedureName, isPublic);
        _procedureCache.AddOrUpdate(procedureName, isPublic, (_, _) => isPublic);
        return Ok(result);
    }

    [HttpPost("{procedureName}")]
    [Consumes("application/json")]
    public async Task<IActionResult> ExecuteProcedure(string procedureName, [FromBody] JsonElement parameters)
    {
        try
        {
            var procedures = await _repository.GetAvailableStoredProceduresAsync();
            var procInfo = procedures.FirstOrDefault(p => p.Name.Equals(procedureName, StringComparison.OrdinalIgnoreCase));

            if (procInfo == null || !procInfo.IsPublic)
                return Ok(new { });

            // Convert parameters based on procedure name
            object procParams;
            if (procedureName.Equals("GetHeroSectionByDate", StringComparison.OrdinalIgnoreCase))
            {
                var dateStr = parameters.GetProperty("p_created_date").GetString();
                if (DateTime.TryParse(dateStr, out DateTime date))
                {
                    procParams = new { p_created_date = date };  // Remove .Date to preserve time component
                }
                else
                {
                    return Ok(new { });  // Return empty for invalid date
                }
            }
            else
            {
                // Convert JsonElement to Dictionary<string, object>
                procParams = JsonSerializer.Deserialize<Dictionary<string, object>>(parameters.GetRawText());
            }
    
            var result = await _repository.ExecuteStoredProcedureAsync<dynamic>(procedureName, procParams);
            return Ok(result);
        }
        catch (Exception)
        {
            return Ok(new { });  // Return empty for any errors
        }
    }

[HttpGet("procedures/{procedureName}/details")]
public async Task<IActionResult> GetProcedureDetails(string procedureName)
{
    try
    {
        var procedures = await _repository.GetAvailableStoredProceduresAsync();
        var procInfo = procedures.FirstOrDefault(p => 
            p.Name.Equals(procedureName, StringComparison.OrdinalIgnoreCase));

        if (procInfo == null || !procInfo.IsPublic)
            return NotFound();

        var parameters = await _repository.GetStoredProcedureParametersAsync(procedureName);
        var sampleResponse = await _repository.ExecuteStoredProcedureWithDefaultsAsync(procedureName);

        var details = new StoredProcedureDetailModel
        {
            Name = procedureName,
            Parameters = parameters,
            SampleResponse = sampleResponse
        };

        return Ok(details);
    }
    catch (Exception ex)
    {
        // Log the error
        return StatusCode(500, new { error = "An error occurred while fetching procedure details." });
    }
}
}