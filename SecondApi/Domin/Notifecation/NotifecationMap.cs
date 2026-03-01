using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SecondApi.Domin.Notifecation
{
    public class NotifecationMap : IEntityTypeConfiguration<Notifecation>
    {
        public void Configure(EntityTypeBuilder<Notifecation> builder)
        {
            builder.ToTable("Notifecations", "dbo");
            builder.HasKey(n => n.Id);
            builder.Property(n => n.Title).IsRequired().HasMaxLength(100);
            builder.Property(n => n.Stutes).IsRequired().HasMaxLength(500);
            builder.Property(n => n.RefuseReason).HasMaxLength(100);
            builder.Property(n => n.CreatedAt);
            builder.Property(n => n.UserId).IsRequired();
        }
    }
}
