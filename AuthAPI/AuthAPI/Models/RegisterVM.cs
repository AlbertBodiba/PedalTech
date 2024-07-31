namespace AuthAPI.Models
{
    public class RegisterVM
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string? Role { get; set; }
        public string Password { get; set; }
        public string Gender { get; set; }
        public string ID_Number { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool AccountStatus { get; set; } // New field
        public string? ResidentialAddress { get; set; } // New field
        public string? ProfileImageBase64 { get; set; } // New field
    }

    public class ForgotPasswordVM
    {
        public string Email { get; set; }
    }

    public class ResetPasswordVM
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string Password { get; set; }
    }
}
