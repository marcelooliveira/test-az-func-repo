using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace VollMed.FunctionApp;

public class GerarProntuarioMedicoV9
{
    private readonly ILogger<GerarProntuarioMedicoV9> _logger;

    public GerarProntuarioMedicoV9(ILogger<GerarProntuarioMedicoV9> logger)
    {
        _logger = logger;
    }

    [Function("GerarProntuarioMedicoV9")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult("Welcome to Azure Functions!");
    }
}