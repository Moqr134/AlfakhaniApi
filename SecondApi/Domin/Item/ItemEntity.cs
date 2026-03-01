using Common.App;
using SecondApi.Domin.Category;
using SecondApi.Domin.Sizes;

namespace SecondApi.Domin.Item
{
    public class ItemEntity:Entity
    {
        public required string ItemName { get; set; }
        public required string Description { get; set; }
        public required int CategoryId { get; set; }
        public required double Price { get; set; }
        public required string Type { get; set; }
        public bool Showing { get; set; }
        public string? ItemImage { get; set; }
        public string? ImageContentType { get; set; }
        public ICollection<SizesEntity>? Sizes { get; set; }
        public CategoryEntity? Category { get; set; }
    }
}
