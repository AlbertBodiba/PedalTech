using Microsoft.AspNetCore.Identity;
using System;

namespace AuthAPI.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string? UserRole { get; set; }
        public string Gender { get; set; }
        public string ID_Number { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool AccountStatus { get; set; }
        public string? ResidentialAddress { get; set; }
        public string? ProfileImageBase64 { get; set; }
    }
}
