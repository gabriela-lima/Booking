using System.Text.Json.Serialization;

namespace BookingS.Models
{
    public class AppointmentSlot
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? PatientName { get; set; }
        public string? Text => PatientName;
        [JsonPropertyName("patient")]
        public string? PatientId { get; set; }
        public string Status { get; set; } = "available";
    }
}
