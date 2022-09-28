using System;
using BookingS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Pomelo.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql;
using Pomelo.EntityFrameworkCore.MySql.Design;
namespace BookingS.Context
{
    public class DoctorDbContext : DbContext
    {
        public DbSet<AppointmentSlot> Appointments { get; set; }
        //public DoctorDbContext(DbContextOptions<DoctorDbContext> options) : base(options) { }
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppointmentSlot>().HasKey(x => x.Id);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string conn = "Server=localhost;Database=Booking;Uid=root;Pwd=mousePad;";
            optionsBuilder.UseMySql(conn, ServerVersion.AutoDetect(conn));
            base.OnConfiguring(optionsBuilder);
        }
    }
}
