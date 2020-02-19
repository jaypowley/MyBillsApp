using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyBills.Data.Contexts;

namespace MyBills.Data.Configurations
{
    public class SqlLogConfiguration : IEntityTypeConfiguration<SqlLog>
    {
        public void Configure(EntityTypeBuilder<SqlLog> builder)
        {
            builder.ToTable("SqlLog");
            builder.HasKey(s => s.Id);
            builder.Property(p => p.Value);
            builder.Property(p => p.CreatedDate);
        }
    }
}