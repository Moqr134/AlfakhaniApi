using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SecondApi.Domin.Bills
{
    public class BillsItemMap:IEntityTypeConfiguration<BillsItem>
    {
        public void Configure(EntityTypeBuilder<BillsItem> builder)
        {
            builder.ToTable("BillsItem", "dbo");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.BillNumper).IsRequired();
            builder.Property(x => x.ItemId).IsRequired();
            builder.Property(x => x.Quantity).IsRequired();
            builder.Property(x => x.Price).IsRequired().HasMaxLength(10);
            builder.Property(x=>x.ItemName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.TotalPrice).IsRequired().HasMaxLength(10);
            builder.HasOne(x => x.Bills)
                .WithMany(x => x.billsItems)
                .HasForeignKey(x => x.BillNumper)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
