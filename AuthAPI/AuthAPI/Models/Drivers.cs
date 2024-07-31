namespace AuthAPI.Models
{
    public class Drivers
    {
        public int DriverId { get; set; }

        public int? UserTransportId { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? PhoneNumber { get; set; }

        public bool LicenseValidity { get; set; }

        public virtual ICollection<License> Licenses { get; set; } = new List<License>();

    }
}
