using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyBills.Core;
using MyBills.Data.Configurations;
using MyBills.Data.Providers;
using MyBills.Domain.Entities;

namespace MyBills.Data.Contexts
{
    public sealed class MyBillsContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MyBillsContext"/> class.
        /// </summary>
        public MyBillsContext()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // A custom provider to log generated SQL calls from Entity Framework to the database
            var lf = new LoggerFactory();
            lf.AddProvider(new EfLogProvider());
            optionsBuilder
                .UseLoggerFactory(lf)
                .UseSqlServer(AppSettings.ConnectionString);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Log> Log { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<UserBill> UserBills { get; set; }
        public DbSet<UserDetail> UserDetails { get; set; }
        public DbSet<Words> Words { get; set; }
        public DbSet<RecurrenceSchedule> UserBillRecurrenceSchedule { get; set; }
        public DbSet<RecurrenceType> RecurrenceType { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Configure default schema
            modelBuilder.HasDefaultSchema("dbo");

            //Map entities to tables
            modelBuilder
                .ApplyConfiguration(new UserConfiguration())
                .ApplyConfiguration(new UserDetailConfiguration())
                .ApplyConfiguration(new WordConfiguration())
                .ApplyConfiguration(new LogConfiguration())
                .ApplyConfiguration(new RecurrenceTypeConfiguration())
                .ApplyConfiguration(new UserBillConfiguration())
                .ApplyConfiguration(new RecurrenceScheduleConfiguration())
                .ApplyConfiguration(new BillConfiguration());

            modelBuilder.Entity<UserDetail>()
                .HasOne(p => p.User)
                .WithMany(b => b.UserDetail)
                .HasForeignKey(p => p.UserId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
