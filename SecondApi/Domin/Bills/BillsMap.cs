using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SecondApi.Domin.Bills
{
    public class BillsMap : IEntityTypeConfiguration<Bills>
    {
        public void Configure(EntityTypeBuilder<Bills> builder)
        {
            builder.ToTable("Bills", "dbo");
            builder.Property(x => x.Id).ValueGeneratedOnAdd().IsRequired();
            builder.HasKey(x => x.BillNumper);
            builder.Property(x => x.DviceToken);
            builder.Property(x => x.TotalAmount);
            builder.Property(x => x.BillLocation);
            builder.Property(x => x.BillName);
            builder.Property(x => x.BillPhoneNumper);
            builder.Property(x => x.Description)
                .HasMaxLength(500);
            builder.Property(x => x.Status)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("Waiting");
            builder.Property(x => x.CreateUserId);
            builder.Property(x => x.UpdateUserId);
            builder.Property(x => x.CreateDate);
            builder.Property(x => x.IsRemoved);
            builder.Property(x => x.RemoveDate);
            builder.Property(x => x.RemoveUserId);
            builder.Property(x => x.UpdateDate);
            builder.Property(x => x.RefuseReason).HasMaxLength(70);
            builder.Property(x => x.Version).IsRowVersion();
        }
    }
}
