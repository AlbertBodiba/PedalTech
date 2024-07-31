using AuthAPI.Data;
using AuthAPI.Models;

namespace AuthAPI.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailModel emailModel);
    }

    public class EmailService : IEmailService
    {
        private readonly AppDbContext _context;

        public EmailService(AppDbContext context)
        {
            _context = context;
        }

        public async Task SendEmailAsync(EmailModel emailModel)
        {
            // Implement your email sending logic here
            // For example, using SMTP or any third-party email service

            // Save the email to the database
            _context.Emails.Add(emailModel);
            await _context.SaveChangesAsync();
        }
    }
}
