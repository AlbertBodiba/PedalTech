namespace AuthAPI.Models
{
    public class BikeDropIndemnity
    {
        public int BikeDropVolunteerId { get; set; }

        public int IndemnityId { get; set; }

        public DateTime? SignedDate { get; set; }

        //public virtual BikeDropVolunteer BikeDropVolunteer { get; set; } = null!;

        //public virtual IndemnityForm Indemnity { get; set; } = null!;
    }
}
