using System;
using System.Data.SqlClient;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;

namespace VollMed.FunctionApp;

public class GerarProntuarioMedico
{
    private readonly ILogger<GerarProntuarioMedico> _logger;
    private readonly string _sqlConnectionString;
    private readonly string _redisConnectionString;
    private readonly ConnectionMultiplexer _redis;
    private const string TemplateProntuario = @"
# Prontuário Médico

Data do Atendimento: {0:dd/MM/yyyy HH:mm}

## 1. Identificação do médico
Nome médico: {1}
CRM: {2}
Especialidade: {3}

## 2. Identificação do Paciente
CPF: {4}
Nome Completo: 
Data de Nascimento: 

## 3. Anamnese
Queixa Principal: 
Medicação em uso:
";

    public GerarProntuarioMedico(ILogger<GerarProntuarioMedico> logger)
    {
        _logger = logger;
        _sqlConnectionString = Environment.GetEnvironmentVariable("SqlConnectionString")!;
        _redisConnectionString = Environment.GetEnvironmentVariable("RedisConnectionString")!;
        _redis = ConnectionMultiplexer.Connect(_redisConnectionString);
    }

    [Function("GerarProntuarioMedico")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "prontuario/{id:int}")] HttpRequest req,
        int id)
    {
        _logger.LogInformation($"Processando prontuário do médico ID {id}");

        var db = _redis.GetDatabase();

        // 1️. Tenta obter do cache
        string? prontuarioJson = await db.StringGetAsync($"consulta:{id}");

        Prontuario? prontuario = null;

        if (!string.IsNullOrEmpty(prontuarioJson))
        {
            prontuario = JsonSerializer.Deserialize<Prontuario>(prontuarioJson);
            _logger.LogInformation("Dados obtidos do cache Redis.");
        }
        else
        {
            _logger.LogInformation("Dados não encontrados no cache. Consultando o banco SQL...");

            using (var conn = new SqlConnection(_sqlConnectionString))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand("SELECT m.Id, m.Nome, m.Crm, m.Especialidade FROM [dbo].[medicos] as m INNER JOIN [dbo].[consultas] as c ON c.MedicoId = m.Id WHERE c.Id = @Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);

                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    prontuario = new Prontuario
                    {
                        MedicoId = reader.GetInt32(0),
                        MedicoNome = reader.GetString(1),
                        MedicoCrm = reader.GetString(2),
                        MedicoEspecialidade = reader.GetInt32(3),
                        ConsultaData = reader.GetDateTime(4),
                        ConsultaPaciente = reader.GetString(5)
                    };

                    // 2️⃣ Salva no cache Redis por 1 hora
                    await db.StringSetAsync(
                        $"consulta:{id}",
                        JsonSerializer.Serialize(prontuario),
                        TimeSpan.FromHours(1));

                    _logger.LogInformation("Dados do prontuário salvos no cache Redis.");
                }
            }
        }

        if (prontuario == null)
        {
            return new NotFoundObjectResult($"Consulta com Id {id} não encontrada.");
        }

        // 2. Montar template do prontuário

        _logger.LogInformation("Prontuário médico gerado:");
        _logger.LogInformation(TemplateProntuario
            , prontuario.ConsultaData
            , prontuario.MedicoNome
            , prontuario.MedicoCrm
            , Enum.GetName(typeof(Especialidade)
            , prontuario.MedicoEspecialidade)
            , prontuario.ConsultaPaciente);

        return new OkObjectResult($"Prontuário do médico {prontuario.MedicoNome} gerado com sucesso (ver logs).");
    }
}

public class Prontuario
{
    public int MedicoId { get; set; }
    public string MedicoNome { get; set; } = string.Empty;
    public string MedicoCrm { get; set; } = string.Empty;
    public int MedicoEspecialidade { get; set; }
    public DateTime ConsultaData { get; set; }
    public string ConsultaPaciente { get; set; } = string.Empty;
}

public enum Especialidade
{
    [Display(Name = "Cardiologia")] Cardiologia = 1,
    [Display(Name = "Neurocirurgia")] Neurocirurgia = 2,
    [Display(Name = "Cirurgia Geral")] CirurgiaGeral = 3,
    [Display(Name = "Pediatria")] Pediatria = 4,
    [Display(Name = "Oncologia")] Oncologia = 5,
    [Display(Name = "Diagnóstico")] Diagnostico = 6
}