using Microsoft.EntityFrameworkCore;
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

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Bill> Bills { get; set; } = null!;
        public DbSet<UserBill> UserBills { get; set; } = null!;
        public DbSet<UserDetail> UserDetails { get; set; } = null!;
        public DbSet<Words> Words { get; set; } = null!;
        public DbSet<RecurrenceSchedule> UserBillRecurrenceSchedule { get; set; } = null!;
        public DbSet<RecurrenceType> RecurrenceType { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Configure default schema
            modelBuilder.HasDefaultSchema("dbo");

            //Map entities to tables
            modelBuilder
                .ApplyConfiguration(new UserConfiguration())
                .ApplyConfiguration(new BillConfiguration())
                .ApplyConfiguration(new UserBillConfiguration())
                .ApplyConfiguration(new UserDetailConfiguration())
                .ApplyConfiguration(new WordConfiguration())
                .ApplyConfiguration(new RecurrenceScheduleConfiguration())
                .ApplyConfiguration(new RecurrenceTypeConfiguration());            

            base.OnModelCreating(modelBuilder);
        }
    }
}