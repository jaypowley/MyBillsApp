using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyBills.Domain.Entities;

namespace MyBills.Data.Configurations
{
    public class UserDetailConfiguration : IEntityTypeConfiguration<UserDetail>
    {
        public void Configure(EntityTypeBuilder<UserDetail> builder)
        {
            builder.ToTable("UserDetails");
            builder.HasKey(s => s.Id);
            builder.Property(p => p.FirstName)
                .HasMaxLength(50)
                .IsRequired(false); // Allow NULL
            builder.Property(p => p.ProfilePicture)
                .IsRequired(false); // Allow NULL
            builder.Property(p => p.UserId)
                .IsRequired(); // UserId should always have a value
        }
    }
}