namespace BookingS.Models
{
    public class AppointmentSlotRange
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool Weekends { get; set; }
    }
}
