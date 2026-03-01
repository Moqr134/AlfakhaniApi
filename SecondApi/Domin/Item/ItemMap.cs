using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SecondApi.Domin.Item
{
    public class ItemMap:IEntityTypeConfiguration<ItemEntity>
    {
        public void Configure(EntityTypeBuilder<ItemEntity> builder)
        {
            builder.ToTable("Item", "dbo");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ItemName).HasMaxLength(50);
            builder.Property(x => x.CategoryId);
            builder.Property(x => x.Price);
            builder.Property(x => x.Type).HasMaxLength(20);
            builder.Property(x => x.Description);
            builder.Property(x => x.ItemImage);
            builder.Property(x => x.ImageContentType);
            builder.Property(x => x.CreateUserId);
            builder.Property(x => x.RemoveUserId);
            builder.Property(x=>x.Showing).HasDefaultValue(true);
            builder.Property(x => x.Version).IsRowVersion();
            builder.Property(x => x.CreateDate);
            builder.Property(x => x.IsRemoved);
            builder.Property(x => x.RemoveDate);
            builder.Property(x => x.UpdateDate);
            builder.Property(x => x.UpdateUserId);
            builder.HasOne(x => x.Category)
                   .WithMany(x => x.Items)
                   .HasForeignKey(x => x.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
