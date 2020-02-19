using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyBills.Domain.Entities;

namespace MyBills.Data.Configurations
{
    public class UserConfiguration: IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Username).HasMaxLength(50);
            builder.Property(c => c.Email).HasMaxLength(50);
            builder.Property(c => c.PasswordHash).HasMaxLength(100);
            builder.Property(c => c.CreatedDate);
            builder.Property(c => c.UpdatedDate);
        }
    }
}
