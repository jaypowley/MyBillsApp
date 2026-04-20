using Microsoft.EntityFrameworkCore;
using MyBills.Core;
using MyBills.Data.Configurations;
using MyBills.Domain.Entities;

namespace MyBills.Data.Contexts
{
    public sealed class MyBillsContext : DbContext
    {
        public MyBillsContext(DbContextOptions<MyBillsContext> options)
        : base(options)
        {
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
                .ApplyConfiguration(new RecurrenceTypeConfiguration())
                .ApplyConfiguration(new UserBillConfiguration())
                .ApplyConfiguration(new RecurrenceScheduleConfiguration())                
                .ApplyConfiguration(new BillConfiguration());

            //modelBuilder.Entity<UserDetail>()
            //    .HasOne(p => p.User)
            //    .WithMany(b => b.UserDetail)
            //    .HasForeignKey(p => p.UserId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
