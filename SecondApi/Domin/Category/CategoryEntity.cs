using Common.App;
using SecondApi.Domin.Item;

namespace SecondApi.Domin.Category
{
    public class CategoryEntity:Entity
    {
        public required string CategoryName { get; set; } 
        public ICollection<ItemEntity>? Items { get; set; }
    }
}
