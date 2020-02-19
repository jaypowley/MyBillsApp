using Microsoft.EntityFrameworkCore;
using System;
using MyBills.Core;
using MyBills.Data.Configurations;

namespace MyBills.Data.Contexts
{
    public sealed class EfLogContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EfLogContext"/> class.
        /// </summary>
        public EfLogContext()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(AppSettings.ConnectionString);
        }

        public DbSet<SqlLog> SqlLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Configure default schema
            modelBuilder.HasDefaultSchema("ent");

            //Map entities to tables
            modelBuilder.ApplyConfiguration(new SqlLogConfiguration());
            
            base.OnModelCreating(modelBuilder);
        }
    }

    public class SqlLog
    {
        public int Id { get; set; }
        public string Value { get; private set; }
        public DateTime CreatedDate { get; private set; }

        public static void LogSql(string value)
        {
            if (string.IsNullOrWhiteSpace(value.Trim()))
            {
                return;
            }

            using var ctx = new EfLogContext();
            ctx.SqlLogs.Add(new SqlLog
            {
                Value = value,
                CreatedDate = DateTime.Now
            });

            ctx.SaveChanges();
        }
    }
}
