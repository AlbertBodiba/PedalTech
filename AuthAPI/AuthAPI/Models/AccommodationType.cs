namespace AuthAPI.Models
{
    public class AccommodationType
    {

        public int AccommodationTypeId { get; set; }

        public int? EventAccommodationId { get; set; }

        public string? AccommodationTypeDescription { get; set; }

        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    }
}
