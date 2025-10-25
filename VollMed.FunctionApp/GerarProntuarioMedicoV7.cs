using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace VollMed.FunctionApp;

public class GerarProntuarioMedicoV7
{
    private readonly ILogger<GerarProntuarioMedicoV7> _logger;

    public GerarProntuarioMedicoV7(ILogger<GerarProntuarioMedicoV7> logger)
    {
        _logger = logger;
    }

    [Function("GerarProntuarioMedicoV7")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult("Welcome to Azure Functions!");
    }
}