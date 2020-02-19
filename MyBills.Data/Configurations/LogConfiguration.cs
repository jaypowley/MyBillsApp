using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyBills.Core;

namespace MyBills.Data.Configurations
{
    public class LogConfiguration : IEntityTypeConfiguration<Log>
    {
        public void Configure(EntityTypeBuilder<Log> builder)
        {
            builder.ToTable("Logs");
            builder.HasKey(s => s.LogId);
            builder.Property(p => p.LogLevel);
            builder.Property(p => p.TimeStamp);
            builder.Property(p => p.CurrentMethod).HasMaxLength(100);
            builder.Property(p => p.ErrorMessage).HasMaxLength(4000);
            builder.Property(p => p.StackTrace);
            builder.Property(p => p.UserName).HasMaxLength(50);
        }
    }
}
