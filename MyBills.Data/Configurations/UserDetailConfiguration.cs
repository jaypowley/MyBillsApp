using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyBills.Domain.Entities;

namespace MyBills.Data.Configurations
{
    public class UserDetailConfiguration: IEntityTypeConfiguration<UserDetail>
    {
        public void Configure(EntityTypeBuilder<UserDetail> builder)
        {
            builder.ToTable("UserDetails");
            builder.HasKey(s => s.Id);
            builder.Property(p => p.FirstName).HasMaxLength(50);
            builder.Property(p => p.ProfilePicture);
            builder.Property(p => p.UserId);
            //builder.HasRequired(p => p.User);
        }
    }
}
