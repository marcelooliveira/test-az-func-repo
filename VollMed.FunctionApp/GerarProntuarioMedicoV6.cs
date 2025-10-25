using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace VollMed.FunctionApp;

public class GerarProntuarioMedicoV6
{
    private readonly ILogger<GerarProntuarioMedicoV6> _logger;

    public GerarProntuarioMedicoV6(ILogger<GerarProntuarioMedicoV6> logger)
    {
        _logger = logger;
    }

    [Function("GerarProntuarioMedicoV6")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult("Welcome to Azure Functions!");
    }
}