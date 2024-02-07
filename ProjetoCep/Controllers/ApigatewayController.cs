// ApigatewayController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

[ApiController]
[Route("apigateway")]
public class ApigatewayController : ControllerBase
{
    private readonly ILogger<ApigatewayController> _logger;
    private readonly ICepService _cepService;

    public ApigatewayController(ILogger<ApigatewayController> logger, ICepService cepService)
    {
        _logger = logger;
        _cepService = cepService;
    }

    [HttpGet("Address/{cep}")]
    public async Task<IActionResult> GetLocations(string cep)
    {
        try
        {
            if (!_cepService.IsValidCep(cep, out var errorMessage))
            {
                return BadRequest(errorMessage);
            }

            var result = await _cepService.GetCepInformation(cep);

            if (result.IsSuccess)
            {
                return Ok(result.UppercaseJsonResponse);
            }
            else
            {
                return BadRequest(result.ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar solicitação de CEP para o CEP: {CEP}", cep);
            return BadRequest($"Erro ao processar solicitação de CEP para o CEP {cep}: {ex.Message}");
        }
    }
}
