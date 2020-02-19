using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyBills.Domain.Entities;

namespace MyBills.Data.Configurations
{
    public class RecurrenceScheduleConfiguration : IEntityTypeConfiguration<RecurrenceSchedule>
    {
        public void Configure(EntityTypeBuilder<RecurrenceSchedule> builder)
        {
            builder.ToTable("RecurrenceSchedules", "dbo");
            builder.HasKey(s => s.Id);
            builder.Property(p => p.RecurrenceTypeId);
            builder.Property(p => p.Schedule).HasMaxLength(200);
        }
    }
}
