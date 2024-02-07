// ICepService.cs
public interface ICepService
{
    bool IsValidCep(string cep, out string errorMessage);
    Task<CepResult> GetCepInformation(string cep);
}