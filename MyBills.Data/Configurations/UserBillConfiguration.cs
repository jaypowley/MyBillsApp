using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyBills.Domain.Entities;

namespace MyBills.Data.Configurations
{
    public class UserBillConfiguration : IEntityTypeConfiguration<UserBill>
    {
        public void Configure(EntityTypeBuilder<UserBill> builder)
        {
            builder.ToTable("UserBills");
            builder.HasKey(s => s.Id);
            builder.Property(p => p.BillId);
            builder.Property(p => p.Day);
            builder.Property(p => p.Month);
            builder.Property(p => p.Year);
            builder.Property(p => p.IsPaid);
            builder.Property(p => p.UserId);
            builder.Property(p => p.RecurrenceScheduleId);
        }
    }
}
