namespace AuthAPI.Models
{
    public class FlaggedUser
    {

        public int FlaggedUserId { get; set; }

        public int? UserId { get; set; }

        public virtual ICollection<FlagType> FlagTypes { get; set; } = new List<FlagType>();


    }
}
