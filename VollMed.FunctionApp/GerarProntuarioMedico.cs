using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace VollMed.FunctionApp;

public class GerarProntuarioMedico
{
    private readonly ILogger<GerarProntuarioMedico> _logger;

    public GerarProntuarioMedico(ILogger<GerarProntuarioMedico> logger)
    {
        _logger = logger;
    }

    [Function("GerarProntuarioMedicoV3")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult("Welcome to Azure Functions!");
    }
}