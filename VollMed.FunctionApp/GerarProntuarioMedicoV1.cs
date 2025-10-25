using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace VollMed.FunctionApp;

public class GerarProntuarioMedicoV1
{
    private readonly ILogger<GerarProntuarioMedicoV1> _logger;

    public GerarProntuarioMedicoV1(ILogger<GerarProntuarioMedicoV1> logger)
    {
        _logger = logger;
    }

    [Function("GerarProntuarioMedicoV1")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult("Welcome to Azure Functions!");
    }
}