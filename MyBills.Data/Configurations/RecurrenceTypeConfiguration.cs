using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyBills.Domain.Entities;

namespace MyBills.Data.Configurations
{
    public class RecurrenceTypeConfiguration : IEntityTypeConfiguration<RecurrenceType>
    {
        public void Configure(EntityTypeBuilder<RecurrenceType> builder)
        {
            builder.ToTable("RecurrenceTypes");
            builder.HasKey(s => s.Id);
            builder.Property(p => p.Name).HasMaxLength(50);
            builder.Property(p => p.Type).HasMaxLength(200);
        }
    }
}