namespace AuthAPI.Models
{
    public class IndemnityForm
    {
        public int IndemnityId { get; set; }

        public int? Idnumber { get; set; }

        public string? EFormAttachment { get; set; }

        public virtual ICollection<BikeDropIndemnity> BikeDropIndemnities { get; set; } = new List<BikeDropIndemnity>();

    }
}
