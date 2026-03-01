using Infrastructure.Logger;
using Infrastructure.ORM;
using Microsoft.EntityFrameworkCore;
using SecondApi.App.ISirvice;
using SecondApi.Domin.Category;
using SecondApi.Domin.Item;
using SecondApi.Infrastructure.Services;
using SherdProject.DTO;

namespace SecondApi.App.Service
{
    public class CategoryService : MasterService , ICategoryService
    {
        public CategoryService(DBContext context):base(context)
        {
        }

        public async Task<List<CategoryEntity>> GetAllCategories(int PageIndex = 1, int PageSize = 1)
        {
            try
            {
                return Context.Categories
                    .Include(c => c.Items!)
                    .ThenInclude(i => i.Sizes)
                    .Where(c => c.IsRemoved == false)
                    .OrderBy(c => c.Id)
                    .Skip((PageIndex - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();
            }
            catch (Exception ex)
            {
                await loger.WriteAsync(ex, "CategoryService => GetAllCategories");
                throw;
            }
        }

        public async Task<List<CategoryDto>> GetCategories()
        {
            try
            {
                return await Context.Categories.Select(c=> new CategoryDto
                {
                    Id = c.Id,
                    CategoryName = c.CategoryName,
                }).ToListAsync();
            }
            catch (Exception ex)
            {
                await loger.WriteAsync(ex, "CategoryService => GetCategories");
                throw;
            }
        }

        public async Task<CategoryEntity> GetCategory(int Id)
        {
            try
            {
                return await Context.Categories.FindAsync(Id);
            }
            catch(Exception ex)
            {
                await loger.WriteAsync(ex, "CategoryService => GetCategory");
                throw;
            }
        }

        public async Task<CategoryEntity> GetCategory(string name)
        {
            try
            {
                return await Context.Categories.Where(x=>x.CategoryName==name&&x.IsRemoved==false).FirstOrDefaultAsync();
            } 
            catch (Exception ex) {
                await loger.WriteAsync(ex, "CategoryService => GetCategory");
                throw;
            }
        }
    }
}
