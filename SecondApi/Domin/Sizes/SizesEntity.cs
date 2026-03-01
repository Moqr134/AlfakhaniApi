using Common.App;
using SecondApi.Domin.Item;

namespace SecondApi.Domin.Sizes;

public class SizesEntity:Entity
{
    public string Name { get; set; } = string.Empty;
    public int ItemId { get; set; }
    public int Price { get; set; }
    public ItemEntity? Item { get; set; }
}
