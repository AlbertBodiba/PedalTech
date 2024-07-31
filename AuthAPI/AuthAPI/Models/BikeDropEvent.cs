namespace AuthAPI.Models
{
    public class BikeDropEvent
    {
        public int BikeDropEventId { get; set; }

        public string? SchoolName { get; set; }

        public DateTime? DateOfDrop { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public string? BikeDropDescription { get; set; }

        public bool BikeDropStatus { get; set; }

        public int? Limit { get; set; }
      //  public ICollection<Meeting> Meetings { get; set; } // Navigation property
    }

}
