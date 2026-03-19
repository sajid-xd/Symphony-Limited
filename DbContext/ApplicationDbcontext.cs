using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Security.Claims;
using SymphonyLimited.Models;
using SymphonyLimited.Models.AdminModel;

namespace SymphonyLimited
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public DbSet<Registration> Registration { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<EntranceExam> EntranceExams { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<FAQ> FAQs { get; set; }
        public DbSet<Centre> Centres { get; set; }
        public DbSet<AboutUs> AboutUs { get; set; }
        public DbSet<AdminLogin> AdminLogins { get; set; }
        public DbSet<ExamEnrollment> ExamEnrollments { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AdminLogin>().HasData(
                new AdminLogin
                {
                    AdminLoginId = 1,
                    UserName = "admin",
                    Password = "admin123",
                }
            );
        }


    }
}
