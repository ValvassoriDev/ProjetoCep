using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net.Http;

namespace ProjetoCep.Controllers
{
    public class ApigatewayController : ControllerBase
    {
        private readonly ILogger<ApigatewayController> _logger;

        public ApigatewayController(ILogger<ApigatewayController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("~/apigateway/Address/{cep}")]
        public IActionResult GetLocations(string cep)
        {
            try
            {
                // Validando o CEP
                if (string.IsNullOrEmpty(cep) || cep.Length != 8 || !cep.All(char.IsDigit))
                {
                    if (string.IsNullOrEmpty(cep))
                    {
                        return BadRequest("CEP não pode ser vazio.");
                    }
                    else if (cep.Any(char.IsLetter))
                    {
                        return BadRequest("CEP não pode conter letras.");
                    }
                    else if (cep.Length > 8)
                    {
                        return BadRequest("CEP não pode ter mais de 8 dígitos.");
                    }
                    else if (cep.Length < 8)
                    {
                        return BadRequest("CEP deve ter 8 dígitos.");
                    }
                    else
                    {
                        // Se houver caracteres especiais
                        return BadRequest("CEP não pode conter caracteres especiais.");
                    }
                }

                // Configurando o cliente HTTP
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://cep.awesomeapi.com.br/");
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    // Construindo a URL com o CEP dinâmico
                    string requestUrl = $"/json/{cep}";

                    // Fazendo uma solicitação GET e aguardando a resposta
                    HttpResponseMessage response = client.GetAsync(requestUrl).Result;

                    // Verificando se a solicitação foi bem-sucedida
                    if (response.IsSuccessStatusCode)
                    {
                        // Lendo o conteúdo da resposta
                        string jsonResponse = response.Content.ReadAsStringAsync().Result;

                        // Convertendo o JSON para maiúsculas
                        string uppercaseJsonResponse = jsonResponse.ToUpper();

                        // Retorna os dados da resposta em formato JSON (convertidos para maiúsculas)
                        return Ok(uppercaseJsonResponse);
                    }
                    else
                    {
                        // Retorna um BadRequest se a solicitação não foi bem-sucedida
                        return BadRequest("Erro na solicitação HTTP");
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Erro na solicitação HTTP para o CEP: {CEP}", cep);
                return BadRequest("Erro na solicitação HTTP");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro desconhecido ao processar solicitação de CEP para o CEP: {CEP}", cep);
                return BadRequest($"Erro ao processar solicitação de CEP para o CEP {cep}: {ex.Message}");
            }
        }
    }
}