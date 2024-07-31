using AuthAPI.Data;
using AuthAPI.Models;
using AuthAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Logging;

namespace AuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _appDbContext;
        private readonly IConfiguration _configuration;
        private readonly IAuth _authRepository;
        private readonly ILogger<AuthController> _logger;
        private readonly AuditLogService _auditLogService;


        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration,
                              RoleManager<IdentityRole> roleManager, AppDbContext appDbContext, IAuth authRepository, ILogger<AuthController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _appDbContext = appDbContext;
            _configuration = configuration;
            _authRepository = authRepository;
            _logger = logger;
        }

        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            return Ok(users);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterVM registerVM)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ValidationProblemDetails(ModelState));
                }

                var user = new User
                {
                    Email = registerVM.Email,
                    Name = registerVM.Name,
                    Surname = registerVM.Surname,
                    UserName = registerVM.Email,
                    UserRole = registerVM.Role,
                    Gender = registerVM.Gender,
                    ID_Number = registerVM.ID_Number,
                    PhoneNumber = registerVM.PhoneNumber,
                    Address = registerVM.Address,
                    DateOfBirth = registerVM.DateOfBirth,
                    AccountStatus = registerVM.AccountStatus,
                    ResidentialAddress = registerVM.ResidentialAddress,
                    ProfileImageBase64 = registerVM.ProfileImageBase64
                };

                var result = await _userManager.CreateAsync(user, registerVM.Password);
                if (!result.Succeeded)
                {
                    return BadRequest(new { message = "User creation failed", errors = result.Errors });
                }
                await _auditLogService.LogActionAsync(user.Id, "Register", "User", user.Id.ToString());
                await _userManager.AddToRoleAsync(user, registerVM.Role);

                await _appDbContext.SaveChangesAsync();

                return Ok(new { message = "User registered successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failed");
                return StatusCode(500, new { message = ex.Message, details = ex.InnerException?.Message });
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> UserLogin(LoginVM model)
        {
            var results = await _authRepository.LoginUserAsync(model);

            if (!results.isSuccess)
            {
                return BadRequest(results);
            }
            else
            {
                return Ok(new { token = results.Message });
            }
        }

        [HttpDelete("DeleteUser/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(new { message = "Delete failed", errors = result.Errors });
            }

            return Ok(new { message = "User deleted successfully" });
        }

        [HttpGet("SeedAdmin")]
        public async Task<IActionResult> SeedAdmin()
        {
            await SeedRoles();

            var admin = new User
            {
                UserName = "johnD@org.com",
                Name = "John",
                Email = "johnD@org.com",
                Surname = "Doe",
                UserRole = "ADMIN",
                Gender = "Male",
                ID_Number = "1234567890",
                PhoneNumber = "1234567890",
                Address = "123 Admin Street",
                DateOfBirth = new DateTime(1980, 1, 1),
                AccountStatus = true // Default to true when seeding admin
            };

            var result = await _userManager.CreateAsync(admin, "John100%");

            if (!result.Succeeded)
            {
                return Ok(new { isSuccess = false, message = result.Errors.ToString() });
            }
            await _userManager.AddToRoleAsync(admin, "ADMIN");

            return Ok(new { isSuccess = true, message = "Admins seeded" });
        }

        private async Task SeedRoles()
        {
            var roles = new[] { "ADMIN", "Volunteer" };
            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "ADMIN")]
        [HttpGet("AdminOnly")]
        public IActionResult AdminOnly()
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            return Ok(new { message = $"Admin Only End Point user {userName}" });
        }

        private async Task<string> GenerateJwtToken(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var roleName = roles.First();
            var claims = new[]
            {
                new Claim(ClaimTypes.Role, roleName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Surname, user.Surname),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));

            var token = new JwtSecurityToken
            (
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(15),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpGet("GetUser/{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            return Ok(user);
        }

        [HttpPut("UpdateProfileImage")]
        public async Task<IActionResult> UpdateProfileImage([FromBody] UpdateProfileImageVM model)
        {
            _logger.LogInformation("UpdateProfileImage called");
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _logger.LogInformation($"User ID from token: {userId}");
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User not found");
                return NotFound(new { message = "User not found" });
            }

            user.ProfileImageBase64 = model.ProfileImageBase64;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                _logger.LogError("Update failed", result.Errors);
                return BadRequest(new { message = "Update failed", errors = result.Errors });
            }

            _logger.LogInformation("Profile image updated successfully");
            return Ok(new { message = "Profile image updated successfully" });
        }

        [HttpPut("UpdatePassword")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordVM model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(new { message = "Password update failed", errors = result.Errors });
            }

            return Ok(new { message = "Password updated successfully" });
        }

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "ADMIN,EMPLOYEE")]
        [HttpPut("UpdateUser/{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserVM model)
        {
            _logger.LogInformation($"Updating user with ID: {id}");
            _logger.LogInformation($"Received model: {System.Text.Json.JsonSerializer.Serialize(model)}");

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            user.Name = model.Name;
            user.Surname = model.Surname;
            user.Email = model.Email;
            user.ProfileImageBase64 = model.ProfileImageBase64; // New field

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(new { message = "Update failed", errors = result.Errors });
            }

            _logger.LogInformation($"Role received: {model.Role}");

            return Ok(new { message = "User updated successfully" });
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordVM model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest(new { message = "User not found" });
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = $"http://localhost:4200/reset-password?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(user.Email)}";

            // Send the reset link via email
            SendResetPasswordEmail(user.Email, resetLink);

            return Ok(new { message = "Password reset link has been sent to your email address" });
        }

        private void SendResetPasswordEmail(string email, string resetLink)
        {
            var emailSettings = _configuration.GetSection("EmailSettings").Get<EmailSettings>();
            var fromAddress = new MailAddress(emailSettings.From, "Your App Name");
            var toAddress = new MailAddress(email);
            string fromPassword = emailSettings.Password; const string subject = "Password Reset Request";
            string body = $"Please reset your password by clicking here: {resetLink}";

            var smtp = new SmtpClient
            {
                Host = emailSettings.SmtpServer,
                Port = emailSettings.Port,
                EnableSsl = emailSettings.UseSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false, // Set to false to use custom credentials
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword) // Use retrieved credentials
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }


        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordVM model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest(new { message = "User not found" });
            }

            _logger.LogInformation($"Attempting to reset password for user: {user.Email} with token: {model.Token}");

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (!result.Succeeded)
            {
                _logger.LogError($"Password reset failed for user: {user.Email} with token: {model.Token}. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                return BadRequest(new { message = "Password reset failed", errors = result.Errors });
            }

            return Ok(new { message = "Password has been reset successfully" });
        }



}

    public class UpdateUserVM
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string DOB { get; set; }
        public string Address { get; set; }
        public string? ProfileImageBase64 { get; set; } // New field
    }

    public class EmailSettings
    {
        public string From { get; set; }
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public bool UseSsl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class UpdateProfileImageVM
    {
        public string ProfileImageBase64 { get; set; }
    }

    public class UpdatePasswordVM
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
    }