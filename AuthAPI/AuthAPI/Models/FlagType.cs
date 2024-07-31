namespace AuthAPI.Models
{
    public class FlagType
    {
        public int FlagTypeId { get; set; }

        public int? FlaggedUserId { get; set; }

        public string? FlagTypeDescription { get; set; }

        public virtual FlaggedUser? FlaggedUser { get; set; }
    }
}
