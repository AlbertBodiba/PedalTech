namespace AuthAPI.Models
{
    public class Accommodation
    {
        public int EventAccommodationId { get; set; }
        public string? AccommodationName { get; set; }

        public string? AccommodationDescription { get; set; }

        //public int? BikeDropEventId { get; set; }

        //public virtual ICollection<AccommodationType> AccommodationTypes { get; set; } = new List<AccommodationType>();

        //public virtual BikeDropEvent? BikeDropEvent { get; set; }
    }
}
