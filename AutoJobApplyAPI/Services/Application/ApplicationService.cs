using AutoJobApplyDatabase.Entities;
using AutoJobApplyDatabase.Repositories;

namespace AutoJobApplyAPI.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IJobRepository _jobRepository;
        private readonly OpenAiService _aiService;
        private readonly EmailService _emailService;

        public ApplicationService(
            IApplicationRepository applicationRepository,
            IUserRepository userRepository,
            IJobRepository jobRepository,
            OpenAiService aiService,
            EmailService emailService)
        {
            _applicationRepository = applicationRepository;
            _userRepository = userRepository;
            _jobRepository = jobRepository;
            _aiService = aiService;
            _emailService = emailService;
        }

        public async Task<Application> ApplyAsync(int userId, int jobId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            var job = await _jobRepository.GetByIdAsync(jobId);

            if (user == null || job == null)
                throw new Exception("Usuário ou vaga não encontrado.");

            var message = await _aiService.GenerateMessage(user, job);
            var result = await _emailService.SendEmail(user.Email, job.Company, job.Title, message, user.CurriculoPath);

            var application = new Application
            {
                UserId = user.Id,
                JobId = job.Id,
                AppliedAt = DateTime.UtcNow,
                MessageSent = message,
                Status = result ? "Enviado" : "Erro"
            };

            await _applicationRepository.AddAsync(application);
            return application;
        }

        public async Task<List<Application>> GetApplicationsByUserAsync(int userId)
        {
            return await _applicationRepository.GetByUserIdAsync(userId);
        }
    }
}
