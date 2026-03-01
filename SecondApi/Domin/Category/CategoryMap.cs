using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SecondApi.Domin.Category
{
    public class CategoryMap : IEntityTypeConfiguration<CategoryEntity>
    {
        public void Configure(EntityTypeBuilder<CategoryEntity> builder)
        {
            builder.ToTable("Category", "dbo");
            builder.HasKey(x => x.Id);
            builder.Property(x=>x.CategoryName).HasMaxLength(50);
            builder.Property(x => x.CreateUserId);
            builder.Property(x => x.RemoveUserId);
            builder.Property(x => x.Version).IsRowVersion();
            builder.Property(x => x.CreateDate);
            builder.Property(x => x.IsRemoved);
            builder.Property(x => x.RemoveDate);
            builder.Property(x => x.RemoveUserId);
            builder.Property(x => x.UpdateDate);
            builder.Property(x => x.UpdateUserId);
            builder.HasMany(x => x.Items)
            .WithOne(x => x.Category)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
