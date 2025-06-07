namespace AutoJobApplyAPI.Services
{
    using AutoJobApplyDatabase.Context;
    using AutoJobApplyDatabase.Entities;
    using HtmlAgilityPack;

    public class ScrapingService
    {
        private readonly AppDbContext _context;

        public ScrapingService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Job>> ScrapeInfoJobsAsync(string termoBusca = "Desenvolvedor")
        {
            var url = $"https://www.infojobs.com.br/vagas-de-{termoBusca.Replace(" ", "-")}.aspx";
            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync(url);

            var vagas = new List<Job>();
            var nodes = doc.DocumentNode.SelectNodes("//article[@class='vaga']");

            if (nodes == null) return vagas;

            foreach (var node in nodes)
            {
                try
                {
                    var title = node.SelectSingleNode(".//a[@class='vagaTitle']").InnerText.Trim();
                    var company = node.SelectSingleNode(".//span[@class='vagaCompany']").InnerText.Trim();
                    var location = node.SelectSingleNode(".//span[@class='vagaLocation']").InnerText.Trim();
                    var desc = node.SelectSingleNode(".//p[@class='vagaDescription']").InnerText.Trim();
                    var link = "https://www.infojobs.com.br" + node.SelectSingleNode(".//a[@class='vagaTitle']").Attributes["href"].Value;

                    var job = new Job
                    {
                        Title = title,
                        Company = company,
                        Location = location,
                        Description = desc,
                        Url = link,
                        DatePosted = DateTime.UtcNow
                    };

                    // Evitar duplicatas simples por título + empresa
                    if (!_context.Jobs.Any(j => j.Title == job.Title && j.Company == job.Company))
                    {
                        vagas.Add(job);
                        _context.Jobs.Add(job);
                    }
                }
                catch { /* pular vaga com erro */ }
            }

            await _context.SaveChangesAsync();
            return vagas;
        }
    }
}
