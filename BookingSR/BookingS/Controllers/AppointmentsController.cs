using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingS.Models;
using System.Collections;
using BookingS.Service;
using BookingS.Context;

namespace BookingS.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly DoctorDbContext _context;
        public AppointmentsController(DoctorDbContext context)
        {
            _context = context;
        }

        //api/Appointments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppointmentSlot>>> GetAppointments([FromQuery] DateTime startTime, [FromQuery] DateTime endTime)
        {
            return await _context.Appointments.Where(x => !((x.EndTime <= startTime) || (x.StartTime >= endTime))).ToListAsync();
        }

        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<AppointmentSlot>>> GetAppointments([FromQuery] DateTime startTime, [FromQuery] DateTime endTime, [FromQuery] string patient)
        {
            return await _context.Appointments.Where(x => (x.Status == "available" || (x.Status != "available" && x.PatientId == patient)) && !((x.EndTime <= startTime) || (x.StartTime >= endTime))).ToListAsync();
        }

        //api/Appointments/id
        [HttpGet("{id}")]
        public async Task<ActionResult<AppointmentSlot>> GetAppointmentSlot(int id)
        {
            var appointmentSlot = await _context.Appointments.FindAsync(id);
            if (appointmentSlot == null)
            {
                return NotFound();
            }
            return appointmentSlot;
        }

        //api/Appointments/id
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAppointmentSlot(int id, AppointmentSlotUpdate update)
        {
            var appointmentSlot = await _context.Appointments.FindAsync(id);
            if (appointmentSlot == null)
            {
                return NotFound();
            }
            appointmentSlot.StartTime = update.StartTime;
            appointmentSlot.EndTime = update.EndTime;
            if (update.Name != null)
            {
                appointmentSlot.PatientName = update.Name;
            }
            if (update.Status != null)
            {
                appointmentSlot.Status = update.Status;
            }
            _context.Appointments.Update(appointmentSlot);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        //api/Appointments/id
        [HttpPut("{id}/request")]
        public async Task<IActionResult> PutAppointmentSlotRequest(int id, AppointmentSlotRequest slotRequest)
        {
            var appointmentSlot = await _context.Appointments.FindAsync(id);
            if (appointmentSlot == null)
            {
                return NotFound();
            }
            appointmentSlot.PatientName = slotRequest.Name;
            appointmentSlot.PatientId = slotRequest.Patient;
            appointmentSlot.Status = "waiting";

            _context.Appointments.Update(appointmentSlot);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        //api/Appointments
        [HttpPost]
        public async Task<ActionResult<AppointmentSlot>> PostAppointmentSlot(AppointmentSlot appointmentSlot)
        {
            _context.Appointments.Add(appointmentSlot);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetAppointmentSlot", new { id = appointmentSlot.Id }, appointmentSlot);
        }

        [HttpPost("create")]
        public async Task<ActionResult<AppointmentSlot>> PostAppointmentSlots(AppointmentSlotRange range)
        {
            var existing = await _context.Appointments.Where(x => !((x.EndTime <= range.StartTime) || (x.StartTime >= range.EndTime))).ToListAsync();
            var slots = Timeline.GenerateSlots(range.StartTime, range.EndTime, range.Weekends);
            slots.ForEach(slot =>
            {
                var overlaps = existing.Any(x => !((x.EndTime <= slot.StartTime) || (x.StartTime >= slot.EndTime)));
                if (overlaps)
                {
                    return;
                }
                _context.Appointments.Add(slot);
            });
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("clear")]
        public async Task<ActionResult<AppointmentSlot>> PostAppointmentClear(ClearRange range)
        {
            var startTime = range.StartTime;
            var endTime = range.EndTime;
            _context.Appointments.RemoveRange(_context.Appointments.Where(x => x.Status == "available" && !((x.EndTime <= startTime) || (x.StartTime >= endTime))));
            await _context.SaveChangesAsync();
            return NoContent();
        }

        //api/Appointments/id
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointmentSlot(int id)
        {
            var appointmentSlot = await _context.Appointments.FindAsync(id);
            if (appointmentSlot == null)
            {
                return NotFound();
            }
            _context.Appointments.Remove(appointmentSlot);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool AppointmentSlotExists(int id)
        {
            return _context.Appointments.Any(x => x.Id == id);
        }
    }
}