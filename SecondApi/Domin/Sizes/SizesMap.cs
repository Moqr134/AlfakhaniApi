using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SecondApi.Domin.Sizes
{
    public class SizesMap : IEntityTypeConfiguration<SizesEntity>
    {
        public void Configure(EntityTypeBuilder<SizesEntity> builder)
        {
            builder.ToTable("Sizes","dbo");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Price);
            builder.Property(x => x.CreateUserId);
            builder.Property(x => x.RemoveUserId);
            builder.Property(x => x.UpdateUserId);
            builder.Property(x => x.CreateDate);
            builder.Property(x => x.IsRemoved);
            builder.Property(x => x.ItemId);
            builder.Property(x => x.Name);
            builder.Property(x => x.RemoveDate);
            builder.Property(x => x.UpdateDate);
            builder.Property(x => x.Version).IsRowVersion();
            builder.HasOne(x => x.Item)
                .WithMany(x => x.Sizes)
                .HasForeignKey(x => x.ItemId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
