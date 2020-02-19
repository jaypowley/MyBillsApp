using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyBills.Domain.Entities;

namespace MyBills.Data.Configurations
{
    public class WordConfiguration: IEntityTypeConfiguration<Words>
    {
        public void Configure(EntityTypeBuilder<Words> builder)
        {
            builder.ToTable("Words");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Word).HasMaxLength(10);
        }
    }
}
