using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domin.User;

public class UserMap: IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users", "dbo");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Username).HasMaxLength(16);
        builder.Property(x => x.HashPassword);
        builder.Property(x => x.Email).HasMaxLength(20);
        builder.Property(x => x.Code).HasMaxLength(7);
        builder.Property(x => x.IsAdmin);
        builder.Property(x => x.IsConfirm);
        builder.Property(x => x.IsOnline);
        builder.Property(x => x.LastLogin);
        builder.Property(x => x.LastLogout);
        builder.Property(x => x.IsActive);
        builder.Property(x => x.IsRemoved);
        builder.Property(x => x.RemoveDate);
        builder.Property(x => x.UpdateDate);
        builder.Property(x => x.Version).IsRowVersion();
        builder.Ignore(x => x.Token);
    }
}
