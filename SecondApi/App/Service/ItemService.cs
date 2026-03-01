using Infrastructure.Logger;
using Infrastructure.ORM;
using Microsoft.EntityFrameworkCore;
using SecondApi.App.ISirvice;
using SecondApi.Domin.Item;
using SecondApi.Infrastructure.Services;
using SherdProject.DTO;

namespace SecondApi.App.Service
{
    public class ItemService : MasterService, IItemService
    {
        public ItemService(DBContext context) : base(context)
        {
        }
        public async Task<List<ItemEntity>> GetAllItems(int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                return await Context.Items
               .OrderBy(u => u.CategoryId)
               .Skip((pageIndex - 1) * pageSize)
               .Take(pageSize)
               .Where(i => i.IsRemoved == false)
               .ToListAsync();
            }
            catch (Exception ex)
            {
                await loger.WriteAsync(ex, "ItemService => GetAllItems");
                throw;
            }
        }

        public async Task<List<ItemDto>> GetAllItems()
        {
            try
            {
                return await Context.Items.Where(i => i.IsRemoved == false)
                    .Select(i => new ItemDto
                    {
                        Id = i.Id,
                        ItemName = i.ItemName,
                        Description = i.Description,
                        ItemImage = i.ItemImage,
                        ImageContentType = i.ImageContentType,
                        Price = i.Price,
                        Type = i.Type,
                        CategoryId = i.CategoryId,
                        Sizes = i.Sizes.Select(s => new SizeDto
                        {
                            Id = s.Id,
                            Name = s.Name,
                            Price = s.Price,
                        }).ToList()
                    })
                    .ToListAsync();
            }catch (Exception ex)
            {
                loger.Write(ex, "ItemService => GetAllItems");
                throw;
            }
        }

        public async Task<List<ItemDto>> GetAllItemsByCategoryId(int categoryId, int pageIndex = 1, int pageSize = 6)
        {
            try
            {
                return await Context.Items
                    .Where(i => i.CategoryId == categoryId && i.IsRemoved == false && i.Showing == true)
                    .OrderBy(i => i.Id)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .Select(i => new ItemDto
                    {
                        Id = i.Id,
                        ItemName = i.ItemName,
                        Description = i.Description,
                        ItemImage = i.ItemImage,
                        ImageContentType = i.ImageContentType,
                        Price = i.Price,
                        Type = i.Type,
                        CategoryId = i.CategoryId,
                        Sizes = i.Sizes.Select(s => new SizeDto
                        {
                            Id = s.Id,
                            Name = s.Name,
                            Price = s.Price,
                        }).ToList()
                    })
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                loger.Write(ex, "ItemService => GetAllItemsByCategoryId");
                throw;
            }
        }

        public async Task<ItemEntity> GetItem(int id)
        {
            try
            {
                return await Context.Items.FindAsync(id);
            }
            catch (Exception ex)
            {
                await loger.WriteAsync(ex, "ItemService => GetItem");
                throw;
            }
        }

        public async Task<ItemEntity> GetItem(string name)
        {
            try
            {
                return await Context.Items
                    .FirstOrDefaultAsync(i => i.ItemName == name && i.IsRemoved == false);
            }
            catch (Exception ex)
            {
                await loger.WriteAsync(ex, "ItemService => GetItem");
                throw;
            }
        }

        public async Task<ItemDto> GetItemDto(int id)
        {
            try
            {
                return await Context.Items.Where(i => i.Id == id && i.IsRemoved == false)
                    .Select(i => new ItemDto
                    {
                        Id = i.Id,
                        ItemName = i.ItemName,
                        Description = i.Description,
                        ItemImage = i.ItemImage,
                        ImageContentType = i.ImageContentType,
                        Price = i.Price,
                        Type = i.Type,
                        CategoryId = i.CategoryId,
                        Sizes = i.Sizes.Select(s => new SizeDto
                        {
                            Id = s.Id,
                            Name = s.Name,
                            Price = s.Price
                        }).ToList()
                    })
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                loger.Write(ex, "ItemService => GetItemDto");
                throw;
            }
        }

        public Task<List<ItemEntity>> GetItemsByName(string name)
        {
            try
            {
                return Context.Items
                    .Where(i => i.ItemName.Contains(name) && i.IsRemoved == false)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                loger.Write(ex, "ItemService => GetItemsByName");
                throw;
            }
        }
    }
}
