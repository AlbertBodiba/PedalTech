namespace AuthAPI.Models
{
    public class Comments
    {
        public int CommentId { get; set; }

        public int? MeetingId { get; set; }

        public string? CommentDescription { get; set; }

        public virtual Meeting? Meeting { get; set; }
    }
}
