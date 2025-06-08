using AutoJobApplyAPI.Models;
using AutoJobApplyAPI.Services.Interface;
using AutoJobApplyDatabase.Entities;
using Microsoft.Extensions.Options;
using System.Text.Json;

public class OpenAiService : IOpenAIService
{
    private readonly HttpClient _http;
    private readonly string _apiKey;

    public OpenAiService(HttpClient http, IOptions<OpenAiSettings> options)
    {
        _http = http;
        _apiKey = options.Value.ApiKey;
    }

    public async Task<string> GenerateMessage(User user, Job job)
    {
        var prompt = $"""
        Crie uma mensagem de candidatura personalizada para a vaga de {job.Title} na empresa {job.Company}. Dados do candidato:
        Nome: {user.Nome} {user.Sobrenome}
        Sobre: {user.Sobre}
        
        Seja formal, breve e mostre interesse pela vaga.
        """;

        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/completions");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);

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
}