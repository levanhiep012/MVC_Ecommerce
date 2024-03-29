using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MVC_Ecommerce.Models;
namespace MVC_Ecommerce
{
    class UsersMap : IEntityTypeConfiguration<Users>
    {
        public void Configure(EntityTypeBuilder<Users> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(e => e.UserId);
            builder.Property(e => e.UserName).IsRequired().HasMaxLength(50);
            builder.Property(e => e.Password).IsRequired().HasMaxLength(255);
            builder.Property(e => e.Email).IsRequired().HasMaxLength(100);
            builder.Property(e => e.FullName).HasMaxLength(100);
            builder.Property(e => e.Address).HasMaxLength(255);
            builder.Property(e => e.PhoneNumber).HasMaxLength(25);
        }
    }
}