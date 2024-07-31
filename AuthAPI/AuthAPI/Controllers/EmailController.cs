using AuthAPI.Models;
using AuthAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost]
        public async Task<IActionResult> SendEmail([FromBody] EmailModel emailModel)
        {
            if (emailModel == null || string.IsNullOrEmpty(emailModel.Body))
            {
                return BadRequest("Email body cannot be empty.");
            }

            await _emailService.SendEmailAsync(emailModel);
            return Ok(new { message = "Email sent successfully and saved to database." });
        }
    }
}
