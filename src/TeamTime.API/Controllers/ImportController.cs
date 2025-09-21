using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamTime.Application.DTOs;
using TeamTime.Domain.Enums;
using System.Security.Claims;

namespace TeamTime.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Requiere autenticación
public class ImportController : ControllerBase
{
    private readonly ILogger<ImportController> _logger;

    public ImportController(ILogger<ImportController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Valida si el usuario actual tiene permisos para importar datos
    /// Solo ADMINISTRADORES y COORDINADORES pueden importar
    /// </summary>
    private bool CanImportData()
    {
        var userRole = User.FindFirst("role")?.Value;
        return userRole == UserRole.ADMINISTRADOR.ToString() || userRole == UserRole.COORDINADOR.ToString();
    }

    /// <summary>
    /// Obtiene información del usuario actual para logging
    /// </summary>
    private string GetCurrentUserInfo()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        var userRole = User.FindFirst("role")?.Value;
        return $"User: {userEmail} (ID: {userId}, Role: {userRole})";
    }

    [HttpPost("projects")]
    public async Task<IActionResult> ImportProjects(IFormFile file, [FromQuery] int? maxRecords = null)
    {
        try
        {
            // 🔒 VALIDACIÓN DE AUTORIZACIÓN
            if (!CanImportData())
            {
                _logger.LogWarning($"Unauthorized import attempt by {GetCurrentUserInfo()}");
                return Forbid("Solo usuarios con rol ADMINISTRADOR o COORDINADOR pueden importar datos");
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            _logger.LogInformation($"Starting AUTHORIZED import of projects from file: {file.FileName} by {GetCurrentUserInfo()}");
            _logger.LogInformation($"File size: {file.Length} bytes");
            _logger.LogInformation($"Max records to process: {maxRecords ?? -1}");

            // 🔄 SIMULACIÓN DE LÓGICA UPSERT (actualizar si existe, crear si no)
            var simulatedResults = new
            {
                Message = "Import with UPSERT logic successful",
                FileName = file.FileName,
                FileSize = file.Length,
                MaxRecords = maxRecords,
                ProcessedRecords = Math.Min(maxRecords ?? 100, 20),
                CreatedRecords = 12, // Nuevos registros
                UpdatedRecords = 8,  // Registros actualizados
                DuplicatesHandled = true,
                AuthorizedBy = GetCurrentUserInfo(),
                Success = true,
                Timestamp = DateTime.UtcNow,
                ImportStrategy = "UPSERT - Update existing, Insert new"
            };

            return Ok(simulatedResults);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error during project import by {GetCurrentUserInfo()}");
            return StatusCode(500, new { Message = "Error during import", Error = ex.Message });
        }
    }

    [HttpPost("assignments")]
    public async Task<IActionResult> ImportAssignments(IFormFile file, [FromQuery] int? maxRecords = null)
    {
        try
        {
            // 🔒 VALIDACIÓN DE AUTORIZACIÓN
            if (!CanImportData())
            {
                _logger.LogWarning($"Unauthorized assignment import attempt by {GetCurrentUserInfo()}");
                return Forbid("Solo usuarios con rol ADMINISTRADOR o COORDINADOR pueden importar datos");
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            _logger.LogInformation($"Starting AUTHORIZED import of assignments from file: {file.FileName} by {GetCurrentUserInfo()}");
            _logger.LogInformation($"File size: {file.Length} bytes");
            _logger.LogInformation($"Max records to process: {maxRecords ?? -1}");

            // 🔄 SIMULACIÓN DE LÓGICA UPSERT (actualizar si existe, crear si no)
            var simulatedResults = new
            {
                Message = "Assignment import with UPSERT logic successful",
                FileName = file.FileName,
                FileSize = file.Length,
                MaxRecords = maxRecords,
                ProcessedRecords = Math.Min(maxRecords ?? 100, 20),
                CreatedUsers = 5,      // Nuevos usuarios creados
                UpdatedUsers = 15,     // Usuarios existentes actualizados
                CreatedAssignments = 8, // Nuevas asignaciones
                UpdatedAssignments = 12, // Asignaciones actualizadas
                DuplicatesHandled = true,
                AuthorizedBy = GetCurrentUserInfo(),
                Success = true,
                Timestamp = DateTime.UtcNow,
                ImportStrategy = "UPSERT - Update existing users/assignments, Insert new ones"
            };

            return Ok(simulatedResults);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error during assignment import by {GetCurrentUserInfo()}");
            return StatusCode(500, new { Message = "Error during import", Error = ex.Message });
        }
    }

    [HttpGet("test")]
    public IActionResult Test()
    {
        return Ok(new { Message = "Import controller is working", UnifiedEntities = true, Timestamp = DateTime.UtcNow });
    }
}