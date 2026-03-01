using SecondApi.Domin.Item;
using SherdProject.DTO;

namespace SecondApi.App.ISirvice
{
    public interface IItemService
    {
        public Task<ItemEntity> GetItem(int id);
        public Task<ItemDto> GetItemDto(int id);
        public Task<ItemEntity> GetItem(string name);
        public Task<List<ItemEntity>> GetAllItems(int pageIndex = 1, int pageSize = 5);
        public Task<List<ItemDto>> GetAllItemsByCategoryId(int categoryId, int pageIndex = 1, int pageSize = 5);
        public Task<List<ItemDto>> GetAllItems();
        public Task<List<ItemEntity>> GetItemsByName(string name);
    }
}
