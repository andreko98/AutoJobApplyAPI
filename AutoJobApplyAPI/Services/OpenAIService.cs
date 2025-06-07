using AutoJobApplyDatabase.Entities;
using System.Text.Json;

namespace AutoJobApplyAPI.Services
{
    public class OpenAiService
    {
        private readonly HttpClient _http;
        private readonly string _apiKey = "SUA_CHAVE_OPENAI"; // ou pegue do appsettings

        public OpenAiService()
        {
            _http = new HttpClient();
        }

        public async Task<string> GenerateMessage(User user, Job job)
        {
            var prompt = $"""
        Crie uma mensagem de candidatura personalizada para a vaga de {job.Title} na empresa {job.Company}. Dados do candidato:
        Nome: {user.Nome} {user.Sobrenome}
        Sobre: {user.Sobre}
        
        Seja formal, breve e mostre interesse pela vaga.
        """;

            var response = await _http.PostAsJsonAsync("https://api.openai.com/v1/completions", new
            {
                model = "text-davinci-003",
                prompt = prompt,
                temperature = 0.7,
                max_tokens = 150
            });

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadFromJsonAsync<JsonElement>();
            return content.GetProperty("choices")[0].GetProperty("text").ToString().Trim();
        }
    }
}
