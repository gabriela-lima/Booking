using BookingS.Models;
using System.Data;

namespace BookingS.Service
{
    public class Timeline
    {
        public static int SlotDurationMinutes = 60;
        public static int MorningShiftStarts = 9;
        public static int MorningShiftEnds = 13;
        public static int AfternoonShiftStarts = 14;
        public static int AfternoonShiftEnds = 18;

        public static List<AppointmentSlot> GenerateSlots(DateTime startTime, DateTime endTime, bool weekends)
        {
            var result = new List<AppointmentSlot>();
            var timeline = GenerateTimeline(startTime, endTime, weekends);
            foreach (var cell in timeline)
            {
                if (startTime <= cell.StartTime && cell.EndTime <= endTime)
                {
                    for (var slotStart = cell.StartTime; slotStart < cell.EndTime; slotStart = slotStart.AddMinutes(SlotDurationMinutes))
                    {
                        var slotEnd = slotStart.AddMinutes(SlotDurationMinutes);
                        var slot = new AppointmentSlot();
                        slot.StartTime = startTime;
                        slot.EndTime = slotEnd;
                        slot.Status = "available";
                        result.Add(slot);
                    }
                }
            }
            return result;
        }

        private static List<TimeCell> GenerateTimeline(DateTime startTime, DateTime endTime, bool weekends)
        {
            var result = new List<TimeCell>();
            var incrementMorning = 1;
            var incrementAfternoon = 1;
            var days = (endTime.Date - startTime.Date).TotalDays;
            if (endTime > endTime.Date)
            {
                days += 1;
            }
            for (var i = 0; i < days; i++)
            {
                var day = startTime.Date.AddDays(i);
                if (!weekends)
                {
                    if (day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday)
                    {
                        continue;
                    }
                }
                for (var x = MorningShiftStarts; x < MorningShiftEnds; x += incrementMorning)
                {
                    var cell = new TimeCell();
                    cell.StartTime = day.AddHours(x);
                    cell.EndTime = day.AddHours(x + incrementMorning);
                    result.Add(cell);
                }
                for (var x = AfternoonShiftStarts; x < AfternoonShiftEnds; x += incrementAfternoon)
                {
                    var cell = new TimeCell();
                    cell.StartTime = day.AddHours(x);
                    cell.EndTime = day.AddHours(x + incrementAfternoon);
                    result.Add(cell);
                }
            }
            return result;
        }
    }
}
