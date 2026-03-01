using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SecondApi.Domin.User
{
    public class UserConnectionMap : IEntityTypeConfiguration<UserConnection>
    {
        public void Configure(EntityTypeBuilder<UserConnection> builder)
        {
            builder.ToTable("UserConnections","dbo");
            builder.HasKey(uc => uc.Id);
            builder.Property(uc=>uc.UserId);
            builder.Property(uc=>uc.ConnectionId);
            builder.Property(uc=>uc.DeviceToken);
            builder.Property(uc => uc.IsActive)
                   .HasDefaultValue(true)
                   .IsRequired();
            builder.Property(uc => uc.role);
        }
    }
}
