using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AuthAPI.Models
{
    public class StationTask
    { 
     [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Title { get; set; }

    [MaxLength(500)]
    public string Description { get; set; }

    [ForeignKey("BikeStation")]
    public int StationId { get; set; }

    public BikeStation BikeStation { get; set; }
}
}
