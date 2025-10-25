using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace VollMed.FunctionApp;

public class GerarProntuarioMedicoV8
{
    private readonly ILogger<GerarProntuarioMedicoV8> _logger;

    public GerarProntuarioMedicoV8(ILogger<GerarProntuarioMedicoV8> logger)
    {
        _logger = logger;
    }

    [Function("GerarProntuarioMedicoV8")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult("Welcome to Azure Functions!");
    }
}