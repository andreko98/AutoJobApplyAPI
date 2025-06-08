using AutoJobApplyAPI.Services.Interface;
using AutoJobApplyDatabase.Entities;
using AutoJobApplyDatabase.Repositories.ApiKey;
using System.Text.Json;

public class ExternalApiService : IExternalApiService
{
    private readonly HttpClient _http; 
    private readonly IApiKeyRepository _apiKeyRepository;

    public ExternalApiService(HttpClient http, IApiKeyRepository apiKeyRepository)
    {
        _http = http;
        _apiKeyRepository = apiKeyRepository;
    }

    public async Task<string> GenerateMessageOpenAI(User user, Job job)
    {
        var openAiKey = _apiKeyRepository.GetApiKey("OpenAI");

        if (openAiKey != null)
        {
            var prompt = $"""
            Crie uma mensagem de candidatura personalizada para a vaga de {job.Title} na empresa {job.Company}. Dados do candidato:
            Nome: {user.Name} {user.LastName}
            Sobre: {user.About}
        
            Seja formal, breve e mostre interesse pela vaga.
            """;

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/completions");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", openAiKey);

            var body = new
            {
                model = "text-davinci-003",
                prompt = prompt,
                temperature = 0.7,
                max_tokens = 150
            };

            request.Content = JsonContent.Create(body);

            var response = await _http.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadFromJsonAsync<JsonElement>();
            return content.GetProperty("choices")[0].GetProperty("text").ToString().Trim();
        }

        throw new Exception("API Key da OpenAI não foi encontrada.");
    }
}