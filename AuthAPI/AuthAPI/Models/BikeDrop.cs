namespace AuthAPI.Models
{
    public class BikeDropSchool
    {
        public int Id { get; set; }
        public string SchoolName { get; set; }
        public DateTime DropDate { get; set; }
    }

    // Models/Meeting.cs
    public class Meeting
    {
        public int MeetingId { get; set; }
        public int BikeDropEventId { get; set; }
        public string MeetingLink { get; set; }
        public DateTime MeetingDate { get; set; }
        public BikeDropEvent BikeDropEvent { get; set; } // Navigation property
    }

}
