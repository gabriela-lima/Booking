namespace BookingS.Models
{
    public class AppointmentSlotUpdate
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Name { get; set; }
        public string? Status { get; set; }
    }
}
