// CepService.cs
using System.Net.Http;
using System.Threading.Tasks;

public class CepService : ICepService
{
    private readonly HttpClient _httpClient;

    public CepService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public bool IsValidCep(string cep, out string errorMessage)
    {
        errorMessage = null;

        if (string.IsNullOrEmpty(cep))
        {
            errorMessage = "CEP não pode ser vazio.";
            return false;
        }

        if (cep.Length != 8 || !cep.All(char.IsDigit))
        {
            if (cep.Any(char.IsLetter))
            {
                errorMessage = "CEP não pode conter letras.";
            }
            else if (cep.Length > 8)
            {
                errorMessage = "CEP não pode ter mais de 8 dígitos.";
            }
            else if (cep.Length < 8)
            {
                errorMessage = "CEP deve ter 8 dígitos.";
            }
            else
            {
                errorMessage = "CEP não pode conter caracteres especiais.";
            }

            return false;
        }

        return true;
    }

    public async Task<CepResult> GetCepInformation(string cep)
    {
        try
        {
            // Construa a URL específica com o CEP
            string requestUrl = $"https://cep.awesomeapi.com.br/json/{cep}";

            // Faça uma solicitação GET ao serviço online
            HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                // Leitura do conteúdo da resposta
                string jsonResponse = await response.Content.ReadAsStringAsync();

                // Converta o JSON para maiúsculas
                string uppercaseJsonResponse = jsonResponse.ToUpper();

                return new CepResult { IsSuccess = true, UppercaseJsonResponse = uppercaseJsonResponse };
            }
            else
            {
                return new CepResult { IsSuccess = false, ErrorMessage = "Erro na solicitação HTTP para o serviço online." };
            }
        }
        catch (HttpRequestException)
        {
            return new CepResult { IsSuccess = false, ErrorMessage = "Erro na solicitação HTTP para o serviço online." };
        }
    }
}
