using AutoJobApplyDatabase.Entities;
using AutoJobApplyDatabase.Repositories;
using HtmlAgilityPack;
using PuppeteerSharp;
using System.Globalization;


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

        public async Task<List<Job>> ScrapeJobsAsync(string search)
        {
            var vagas = new List<Job>();

            vagas.AddRange(await ScrapeInfoJobsAsync(search));
            vagas.AddRange(await ScrapeVagasComBrAsync(search));
            vagas.AddRange(await ScrapeEmpregosComBrAsync(search));
            vagas.AddRange(await ScrapeJoobleAsync(search)); //Falta corrigir o Jooble, pois não está retornando as vagas corretamente

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

        private async Task<List<Job>> ScrapeInfoJobsAsync(string search)
        {
            var vagas = new List<Job>();
            var url = $"https://www.infojobs.com.br/empregos.aspx?palabra={Uri.EscapeDataString(search)}";

            var executablePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
            if (!System.IO.File.Exists(executablePath))
            {
                Console.WriteLine("Navegador Chrome não encontrado no caminho especificado: " + executablePath);
                return vagas;
            }

            var launchOptions = new LaunchOptions
            {
                Headless = true,
                ExecutablePath = executablePath
            };

            using (var browser = await Puppeteer.LaunchAsync(launchOptions))
            using (var page = await browser.NewPageAsync())
            {
                try
                {
                    await page.GoToAsync(url, new NavigationOptions
                    {
                        WaitUntil = new[] { WaitUntilNavigation.DOMContentLoaded },
                        Timeout = 30000
                    });
                }
                catch (PuppeteerSharp.NavigationException nex)
                {
                    Console.WriteLine("Timeout usando DOMContentLoaded: " + nex.Message);
                    await page.GoToAsync(url, new NavigationOptions
                    {
                        WaitUntil = new[] { WaitUntilNavigation.Networkidle2 },
                        Timeout = 30000
                    });
                }

                await page.WaitForSelectorAsync("div.js_cardLink");

                var content = await page.GetContentAsync();
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(content);

                var jobNodes = doc.DocumentNode.SelectNodes("//div[contains(@class, 'js_cardLink')]");
                if (jobNodes != null)
                {
                    foreach (var node in jobNodes)
                    {
                        try
                        {
                            var dataId = node.GetAttributeValue("data-id", "");
                            if (string.IsNullOrEmpty(dataId))
                                continue;
                            int id = int.TryParse(dataId, out int parsedId) ? parsedId : 0;

                            var jobUrl = $"https://www.infojobs.com.br/empregos.aspx?palabra={Uri.EscapeDataString(search)}&iv={dataId}";

                            var titleNode = node.SelectSingleNode(".//h2[contains(@class, 'font-weight-bold')]");
                            var title = titleNode?.InnerText.Trim();

                            var companyNode = node.SelectSingleNode(".//a[contains(@href, 'empresa-')]");
                            var company = companyNode?.InnerText.Trim();

                            var locationNode = node.SelectSingleNode(".//div[contains(@class, 'mr-24')]");
                            var location = locationNode?.InnerText.Trim();
                            if (!string.IsNullOrEmpty(location))
                            {
                                var commaIndex = location.IndexOf(',');
                                if (commaIndex > 0)
                                {
                                    location = location.Substring(0, commaIndex).Trim();
                                }
                            }

                            DateTime datePosted = DateTime.UtcNow;
                            var dateNode = node.SelectSingleNode(".//div[contains(@class, 'js_date')]");
                            if (dateNode != null)
                            {
                                var dateStr = dateNode.GetAttributeValue("data-value", "");
                                if (!string.IsNullOrEmpty(dateStr) && DateTime.TryParse(dateStr, out DateTime parsedDate))
                                {
                                    datePosted = parsedDate;
                                }
                            }

                            var descriptionListingNode = node.SelectSingleNode(".//div[contains(@class, 'small text-medium')]");
                            var summaryDescription = descriptionListingNode?.InnerText.Trim();

                            string fullDescription = summaryDescription;
                            try
                            {
                                using (var detailPage = await browser.NewPageAsync())
                                {
                                    await detailPage.GoToAsync(jobUrl, WaitUntilNavigation.Networkidle0);
                                    await detailPage.WaitForSelectorAsync("div.pt-24.text-medium.js_vacancyDataPanels.js_applyVacancyHidden");

                                    var detailContent = await detailPage.GetContentAsync();
                                    var detailDoc = new HtmlAgilityPack.HtmlDocument();
                                    detailDoc.LoadHtml(detailContent);

                                    var detailDescNode = detailDoc.DocumentNode.SelectSingleNode("//div[contains(@class, 'pt-24') and contains(@class, 'js_vacancyDataPanels')]");
                                    if (detailDescNode != null)
                                    {
                                        fullDescription = detailDescNode.InnerText.Trim();
                                    }
                                    await detailPage.CloseAsync();
                                }
                            }
                            catch (Exception exDetail)
                            {
                                Console.WriteLine($"Erro ao extrair descrição detalhada para a vaga {dataId}: {exDetail}");
                            }

                            if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(company))
                            {
                                vagas.Add(new Job
                                {
                                    Id = id,
                                    Title = title,
                                    Company = company,
                                    Location = location,
                                    Description = fullDescription,
                                    Url = jobUrl,
                                    DatePosted = datePosted
                                });
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Erro ao processar vaga: " + ex);
                        }
                    }
                }
            }

            return vagas;
        }

        private async Task<List<Job>> ScrapeVagasComBrAsync(string search)
        {
            var vagas = new List<Job>();
            var url = $"https://www.vagas.com.br/vagas-de-{Uri.EscapeDataString(search)}";

            var executablePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
            if (!System.IO.File.Exists(executablePath))
            {
                Console.WriteLine("Navegador Chrome não encontrado no caminho especificado: " + executablePath);
                return vagas;
            }

            var launchOptions = new LaunchOptions
            {
                Headless = true,
                ExecutablePath = executablePath
            };

            using (var browser = await Puppeteer.LaunchAsync(launchOptions))
            using (var page = await browser.NewPageAsync())
            {
                try
                {
                    await page.GoToAsync(url, new NavigationOptions
                    {
                        WaitUntil = new[] { WaitUntilNavigation.DOMContentLoaded },
                        Timeout = 30000
                    });
                }
                catch (PuppeteerSharp.NavigationException nex)
                {
                    Console.WriteLine("Timeout usando DOMContentLoaded: " + nex.Message);
                    await page.GoToAsync(url, new NavigationOptions
                    {
                        WaitUntil = new[] { WaitUntilNavigation.Networkidle2 },
                        Timeout = 30000
                    });
                }

                try
                {
                    await page.WaitForSelectorAsync("li.vaga", new WaitForSelectorOptions { Timeout = 15000 });
                }
                catch (PuppeteerSharp.WaitTaskTimeoutException)
                {
                    Console.WriteLine("Não foi possível localizar os elementos <li> com classe 'vaga'.");
                    return vagas;
                }

                var content = await page.GetContentAsync();
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(content);

                var jobNodes = doc.DocumentNode.SelectNodes("//li[contains(@class, 'vaga')]");
                if (jobNodes == null)
                    return vagas;

                foreach (var node in jobNodes)
                {
                    try
                    {
                        var titleNode = node.SelectSingleNode(".//h2[contains(@class, 'cargo')]/a[contains(@class, 'link-detalhes-vaga')]");
                        if (titleNode == null)
                            continue;

                        var title = HtmlEntity.DeEntitize(titleNode.InnerText.Trim());

                        var companyNode = node.SelectSingleNode(".//span[contains(@class, 'emprVaga')]");
                        var company = companyNode?.InnerText.Trim();

                        var descriptionNode = node.SelectSingleNode(".//div[contains(@class, 'detalhes')]/p");
                        var description = descriptionNode?.InnerText.Trim();

                        var locationNode = node.SelectSingleNode(".//footer//span[contains(@class, 'vaga-local')]");
                        var location = locationNode?.InnerText.Trim();

                        var dateNode = node.SelectSingleNode(".//footer//span[contains(@class, 'data-publicacao')]");
                        var dateText = dateNode?.InnerText.Trim();
                        if (!string.IsNullOrEmpty(dateText))
                        {
                            var match = System.Text.RegularExpressions.Regex.Match(dateText, @"\d{2}/\d{2}/\d{4}");
                            if (match.Success)
                            {
                                dateText = match.Value;
                            }
                        }
                        DateTime datePosted = DateTime.UtcNow;
                        if (!string.IsNullOrEmpty(dateText) &&
                            DateTime.TryParseExact(dateText, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture,
                                                     System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
                        {
                            datePosted = parsedDate;
                        }

                        var link = titleNode.GetAttributeValue("href", "").Trim();
                        if (!string.IsNullOrEmpty(link) &&
                            !link.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                        {
                            link = "https://www.vagas.com.br" + link;
                        }

                        var idStr = titleNode.GetAttributeValue("data-id-vaga", "").Trim();
                        int jobId = 0;
                        if (!string.IsNullOrEmpty(idStr))
                            int.TryParse(idStr, out jobId);

                        if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(company))
                        {
                            vagas.Add(new Job
                            {
                                Id = jobId,
                                Title = title,
                                Company = company,
                                Location = location,
                                Description = description,
                                Url = link,
                                DatePosted = datePosted
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Erro ao processar vaga: " + ex.Message);
                        continue;
                    }
                }
            }

            return vagas;
        }

        private async Task<List<Job>> ScrapeEmpregosComBrAsync(string search)
        {
            var vagas = new List<Job>();
            var url = $"https://www.empregos.com.br/vagas/{Uri.EscapeDataString(search)}";

            var executablePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
            if (!System.IO.File.Exists(executablePath))
            {
                Console.WriteLine("Navegador Chrome não encontrado no caminho especificado: " + executablePath);
                return vagas;
            }

            var launchOptions = new LaunchOptions
            {
                Headless = true,
                ExecutablePath = executablePath
            };

            using (var browser = await Puppeteer.LaunchAsync(launchOptions))
            using (var page = await browser.NewPageAsync())
            {
                try
                {
                    await page.GoToAsync(url, new NavigationOptions
                    {
                        WaitUntil = new[] { WaitUntilNavigation.DOMContentLoaded },
                        Timeout = 30000
                    });
                }
                catch (PuppeteerSharp.NavigationException nex)
                {
                    Console.WriteLine("Timeout usando DOMContentLoaded: " + nex.Message);
                    await page.GoToAsync(url, new NavigationOptions
                    {
                        WaitUntil = new[] { WaitUntilNavigation.Networkidle2 },
                        Timeout = 30000
                    });
                }

                try
                {
                    await page.WaitForSelectorAsync("div#job-card", new WaitForSelectorOptions { Timeout = 15000 });
                }
                catch (PuppeteerSharp.WaitTaskTimeoutException)
                {
                    Console.WriteLine("Elemento div#job-card não encontrado.");
                    return vagas;
                }

                var content = await page.GetContentAsync();
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(content);

                var jobNodes = doc.DocumentNode.SelectNodes("//div[@id='job-card']");
                if (jobNodes == null)
                    return vagas;


                foreach (var node in jobNodes)
                {
                    try
                    {
                        var headerNode = node.SelectSingleNode(".//div[contains(@class, 'flex items-start gap-3')]");
                        if (headerNode == null)
                            continue;

                        var h2Node = headerNode.SelectSingleNode(".//h2");
                        var title = h2Node?.InnerText.Trim();

                        string link = null;
                        var linkNode = h2Node?.SelectSingleNode(".//a");
                        if (linkNode != null)
                        {
                            link = linkNode.GetAttributeValue("href", "").Trim();
                            if (!string.IsNullOrEmpty(link) && !link.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                            {
                                link = "https://www.empregos.com.br" + link;
                            }
                        }

                        var h3Node = headerNode.SelectSingleNode(".//h3");
                        var company = h3Node?.InnerText.Trim();

                        string location = null;
                        var locationNode = node.SelectSingleNode(".//div[span[contains(@class, 'location-on-outline')]]/p");
                        if (locationNode != null)
                        {
                            location = locationNode.InnerText.Trim();
                        }

                        DateTime datePosted = DateTime.UtcNow;
                        var dateNode = node.SelectSingleNode(".//div[span[contains(@class, 'iconify') and contains(@class,'event-outline') and contains(@class, 'cursor-pointer')]]/h3");
                        if (dateNode != null)
                        {
                            string dateText = dateNode.InnerText.Trim();
                            // Exemplo de dateText: "Publicada há 30 dias"
                            var match = System.Text.RegularExpressions.Regex.Match(dateText, @"\d+");
                            if (match.Success && int.TryParse(match.Value, out int days))
                            {
                                datePosted = DateTime.UtcNow.AddDays(-days);
                            }
                        }

                        var descriptionNode = node.SelectSingleNode(".//div[contains(@class, 'mt-2')]/div[contains(@class, 'cursor-pointer')]");
                        var description = descriptionNode?.InnerText.Trim();

                        if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(company))
                        {
                            vagas.Add(new Job
                            {
                                Title = title,
                                Company = company,
                                Location = location,
                                Url = link,
                                DatePosted = datePosted,
                                Description = description
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Erro ao processar vaga: " + ex.Message);
                        continue;
                    }
                }
            }

            return vagas;
        }

        private async Task<List<Job>> ScrapeJoobleAsync(string search)
        {
            var vagas = new List<Job>();
            try
            {
                var url = $"https://br.jooble.org/Vagas?q={Uri.EscapeDataString(search)}";
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

        public async Task<List<Job>> GetRecentJobsAsync(int count)
        {
            return await _jobRepository.GetRecentJobsAsync(count);
        }
    }
}
