using SecondApi.Domin.Category;
using SherdProject.DTO;

namespace SecondApi.App.ISirvice
{
    public interface ICategoryService
    {
        public Task<CategoryEntity> GetCategory(int Id);
        public Task<CategoryEntity> GetCategory(string name);
        public Task<List<CategoryEntity>> GetAllCategories(int PageIndex=1,int PageSize=1);
        public Task<List<CategoryDto>> GetCategories();
    }
}
