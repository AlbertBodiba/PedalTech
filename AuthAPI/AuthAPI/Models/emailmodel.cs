using System.ComponentModel.DataAnnotations;

namespace AuthAPI.Models
{
    // Models/EmailModel.cs
        public class EmailModel
        {
        [Key]
        public int Id { get; set; }
        public string Body { get; set; }
    }
    }


