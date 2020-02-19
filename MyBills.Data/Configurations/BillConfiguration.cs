using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyBills.Domain.Entities;

namespace MyBills.Data.Configurations
{
    public class BillConfiguration: IEntityTypeConfiguration<Bill>
    {
        public void Configure(EntityTypeBuilder<Bill> builder)
        {
            builder.ToTable("Bills");
            builder.HasKey(s => s.Id);
            builder.Property(p => p.Name).HasMaxLength(50);
            builder.Property(p => p.Amount);
            builder.Property(p => p.IsComplete);
            builder.Property(p => p.IsAutoPaid);
        }
    }
}
