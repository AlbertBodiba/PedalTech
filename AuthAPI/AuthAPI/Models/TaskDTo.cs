using System.ComponentModel.DataAnnotations;

namespace AuthAPI.Models
{
    public class TaskDto
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string StationName { get; set; } 
    }
}
