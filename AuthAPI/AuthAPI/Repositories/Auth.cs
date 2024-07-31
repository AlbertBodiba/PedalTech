using AuthAPI.Data;
using AuthAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthAPI.Repositories
{
    public class Auth : IAuth
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _appDbContext;

        public Auth(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration,
                    RoleManager<IdentityRole> roleManager, AppDbContext appDbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _roleManager = roleManager;
            _appDbContext = appDbContext;
        }

        public async Task<ResponseVM> LoginUserAsync(LoginVM loginVM)
        {
            var user = await _userManager.FindByEmailAsync(loginVM.Email);
            if (user == null)
            {
                return new ResponseVM
                {
                    Message = "There is no user with this email",
                    isSuccess = false,
                };
            }
            else
            {
                var result = await _userManager.CheckPasswordAsync(user, loginVM.Password);
                if (!result)
                {
                    return new ResponseVM
                    {
                        Message = "Password is not valid",
                        isSuccess = false
                    };
                }
                else
                {
                    var userRoles = await _userManager.GetRolesAsync(user);

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.Name),
                        new Claim(ClaimTypes.Surname, user.Surname),
                    };

                    foreach (var role in userRoles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));

                    var token = new JwtSecurityToken
                    (
                        issuer: _configuration["Jwt:Issuer"],
                        audience: _configuration["Jwt:Audience"],
                        claims: claims,
                        expires: DateTime.Now.AddDays(30),
                        signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                    );

                    string tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                    return new ResponseVM
                    {
                        Message = tokenString,
                        isSuccess = true,
                    };
                }
            }
        }

        public async Task<ResponseVM> RegisterUserAsync(RegisterVM registerVM)
        {
            var roles = new[] { "ADMIN", "Volunteer" };
            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var newUser = new User
            {
                UserName = registerVM.Email,
                Email = registerVM.Email,
                Name = registerVM.Name,
                Surname = registerVM.Surname,
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

            var existingUsername = await _appDbContext.Users.AnyAsync(u => u.UserName == newUser.UserName);
            var existingEmail = await _appDbContext.Users.AnyAsync(u => u.Email == newUser.Email);

            if (existingUsername || existingEmail)
            {
                return new ResponseVM
                {
                    Message = "Existing user with this email",
                    isSuccess = false,
                };
            }

            var result = await _userManager.CreateAsync(newUser, registerVM.Password);
            if (!result.Succeeded)
            {
                return new ResponseVM
                {
                    Message = "User creation failed.",
                    isSuccess = false,
                };
            }

            await _userManager.AddToRoleAsync(newUser, registerVM.Role);

            return new ResponseVM
            {
                Message = "User created successfully! Please check your email for confirmation.",
                isSuccess = true,
            };
        }
    }
}
