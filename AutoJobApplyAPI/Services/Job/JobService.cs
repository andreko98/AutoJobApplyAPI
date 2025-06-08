using AutoJobApplyDatabase.Entities;
using AutoJobApplyDatabase.Repositories;
using HtmlAgilityPack;

namespace AutoJobApplyAPI.Services
{
    public class JobService : IJobService
    {
        private readonly IJobRepository _jobRepository;

        public JobService(IJobRepository jobRepository)
        {
            _jobRepository = jobRepository;
        }

        public async Task<List<Job>> GetJobsAsync()
        {
            return await _jobRepository.GetAllOrderedByDateDescAsync();
        }

        public async Task<List<Job>> ScrapeJobsAsync(string termo)
        {
            var vagas = new List<Job>();

            vagas.AddRange(await ScrapeInfoJobsAsync(termo));
            vagas.AddRange(await ScrapeVagasComBrAsync(termo));
            vagas.AddRange(await ScrapeIndeedAsync(termo));
            vagas.AddRange(await ScrapeCathoAsync(termo));
            vagas.AddRange(await ScrapeEmpregosComBrAsync(termo));
            vagas.AddRange(await ScrapeJoobleAsync(termo));

            // Remove duplicatas antes de salvar
            var vagasParaAdicionar = new List<Job>();

            foreach (var vaga in vagas)
            {
                bool existe = await _jobRepository.ExistsAsync(vaga.Title, vaga.Company, vaga.Location);
                if (!existe)
                    vagasParaAdicionar.Add(vaga);
            }

            if (vagasParaAdicionar.Count > 0)
                await _jobRepository.AddRangeAsync(vagasParaAdicionar);

            return vagasParaAdicionar;
        }

        private async Task<List<Job>> ScrapeInfoJobsAsync(string termo)
        {
            var vagas = new List<Job>();
            var url = $"https://www.infojobs.com.br/empregos.aspx?palabra={Uri.EscapeDataString(termo)}";

            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync(url);

            var jobNodes = doc.DocumentNode.SelectNodes("//div[contains(@class,'vaga')]");
            if (jobNodes == null) return vagas;

            foreach (var node in jobNodes)
            {
                try
                {
                    var title = node.SelectSingleNode(".//a[@class='vagaTitle']")?.InnerText.Trim();
                    var company = node.SelectSingleNode(".//div[@class='vagaCompany']")?.InnerText.Trim();
                    var location = node.SelectSingleNode(".//div[@class='vagaLocation']")?.InnerText.Trim();
                    var link = node.SelectSingleNode(".//a[@class='vagaTitle']")?.GetAttributeValue("href", "");

                    if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(company))
                    {
                        vagas.Add(new Job
                        {
                            Title = title,
                            Company = company,
                            Location = location,
                            Url = "https://www.infojobs.com.br" + link,
                            DatePosted = DateTime.UtcNow
                        });
                    }
                }
                catch { continue; }
            }

            return vagas;
        }

        private async Task<List<Job>> ScrapeVagasComBrAsync(string termo)
        {
            var vagas = new List<Job>();
            var url = $"https://www.vagas.com.br/vagas-de-{Uri.EscapeDataString(termo)}";

            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync(url);

            var jobNodes = doc.DocumentNode.SelectNodes("//div[contains(@class,'vaga')]");
            if (jobNodes == null) return vagas;

            foreach (var node in jobNodes)
            {
                try
                {
                    var title = node.SelectSingleNode(".//a[@class='vaga_link']")?.InnerText.Trim();
                    var company = node.SelectSingleNode(".//span[@class='empresa']")?.InnerText.Trim();
                    var location = node.SelectSingleNode(".//span[@class='localizacao']")?.InnerText.Trim();
                    var link = node.SelectSingleNode(".//a[@class='vaga_link']")?.GetAttributeValue("href", "");

                    if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(company))
                    {
                        vagas.Add(new Job
                        {
                            Title = title,
                            Company = company,
                            Location = location,
                            Url = "https://www.vagas.com.br" + link,
                            DatePosted = DateTime.UtcNow
                        });
                    }
                }
                catch { continue; }
            }

            return vagas;
        }

        private async Task<List<Job>> ScrapeIndeedAsync(string termo)
        {
            var vagas = new List<Job>();
            try
            {
                var url = $"https://br.indeed.com/jobs?q={Uri.EscapeDataString(termo)}&l=";
                var web = new HtmlWeb();
                var doc = await web.LoadFromWebAsync(url);

                var jobCards = doc.DocumentNode.SelectNodes("//div[contains(@class,'job_seen_beacon')]");

                if (jobCards == null) return vagas;

                foreach (var node in jobCards)
                {
                    try
                    {
                        var title = node.SelectSingleNode(".//h2//span")?.InnerText.Trim();
                        var company = node.SelectSingleNode(".//span[@class='companyName']")?.InnerText.Trim();
                        var location = node.SelectSingleNode(".//div[@class='companyLocation']")?.InnerText.Trim();
                        var linkNode = node.SelectSingleNode(".//h2//a");
                        var link = linkNode?.GetAttributeValue("href", "");

                        if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(company))
                        {
                            vagas.Add(new Job
                            {
                                Title = title,
                                Company = company,
                                Location = location,
                                Url = link?.StartsWith("http") == true ? link : "https://br.indeed.com" + link,
                                DatePosted = DateTime.UtcNow
                            });
                        }
                    }
                    catch { continue; }
                }
            }
            catch { }

            return vagas;
        }

        private async Task<List<Job>> ScrapeCathoAsync(string termo)
        {
            var vagas = new List<Job>();
            try
            {
                var url = $"https://www.catho.com.br/vagas/?q={Uri.EscapeDataString(termo)}";
                var web = new HtmlWeb();
                var doc = await web.LoadFromWebAsync(url);

                var jobCards = doc.DocumentNode.SelectNodes("//div[contains(@class,'job-result-item')]");

                if (jobCards == null) return vagas;

                foreach (var node in jobCards)
                {
                    try
                    {
                        var title = node.SelectSingleNode(".//h2//a")?.InnerText.Trim();
                        var company = node.SelectSingleNode(".//div[contains(@class,'company-name')]")?.InnerText.Trim();
                        var location = node.SelectSingleNode(".//div[contains(@class,'location')]")?.InnerText.Trim();
                        var link = node.SelectSingleNode(".//h2//a")?.GetAttributeValue("href", "");

                        if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(company))
                        {
                            vagas.Add(new Job
                            {
                                Title = title,
                                Company = company,
                                Location = location,
                                Url = link?.StartsWith("http") == true ? link : "https://www.catho.com.br" + link,
                                DatePosted = DateTime.UtcNow
                            });
                        }
                    }
                    catch { continue; }
                }
            }
            catch { }

            return vagas;
        }

        private async Task<List<Job>> ScrapeEmpregosComBrAsync(string termo)
        {
            var vagas = new List<Job>();
            try
            {
                var url = $"https://www.empregos.com.br/vagas?palavra={Uri.EscapeDataString(termo)}";
                var web = new HtmlWeb();
                var doc = await web.LoadFromWebAsync(url);

                var jobCards = doc.DocumentNode.SelectNodes("//div[contains(@class,'vaga-item')]");

                if (jobCards == null) return vagas;

                foreach (var node in jobCards)
                {
                    try
                    {
                        var title = node.SelectSingleNode(".//a[@class='vaga-titulo']")?.InnerText.Trim();
                        var company = node.SelectSingleNode(".//div[@class='vaga-empresa']")?.InnerText.Trim();
                        var location = node.SelectSingleNode(".//div[@class='vaga-localizacao']")?.InnerText.Trim();
                        var link = node.SelectSingleNode(".//a[@class='vaga-titulo']")?.GetAttributeValue("href", "");

                        if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(company))
                        {
                            vagas.Add(new Job
                            {
                                Title = title,
                                Company = company,
                                Location = location,
                                Url = link?.StartsWith("http") == true ? link : "https://www.empregos.com.br" + link,
                                DatePosted = DateTime.UtcNow
                            });
                        }
                    }
                    catch { continue; }
                }
            }
            catch { }

            return vagas;
        }

        private async Task<List<Job>> ScrapeJoobleAsync(string termo)
        {
            var vagas = new List<Job>();
            try
            {
                var url = $"https://br.jooble.org/Vagas?q={Uri.EscapeDataString(termo)}";
                var web = new HtmlWeb();
                var doc = await web.LoadFromWebAsync(url);

                var jobCards = doc.DocumentNode.SelectNodes("//article[contains(@class,'result')]");

                if (jobCards == null) return vagas;

                foreach (var node in jobCards)
                {
                    try
                    {
                        var title = node.SelectSingleNode(".//a[contains(@class,'position')]")?.InnerText.Trim();
                        var company = node.SelectSingleNode(".//div[contains(@class,'company')]")?.InnerText.Trim();
                        var location = node.SelectSingleNode(".//div[contains(@class,'location')]")?.InnerText.Trim();
                        var link = node.SelectSingleNode(".//a[contains(@class,'position')]")?.GetAttributeValue("href", "");

                        if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(company))
                        {
                            vagas.Add(new Job
                            {
                                Title = title,
                                Company = company,
                                Location = location,
                                Url = link?.StartsWith("http") == true ? link : "https://br.jooble.org" + link,
                                DatePosted = DateTime.UtcNow
                            });
                        }
                    }
                    catch { continue; }
                }
            }
            catch { }

            return vagas;
        }
    }
}
