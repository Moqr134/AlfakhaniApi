using App.ISirvice;
using Infrastructure.Cache;
using Infrastructure.ORM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecondApi.App.ISirvice;
using SecondApi.Domin.Item;
using SherdProject.DTO;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SecondApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : MasterController
    {
        private readonly IWebHostEnvironment _env;
        public ItemController(IUserService userService, IAppMemoryCache cache, DBContext context, IItemService itemService,IWebHostEnvironment environment) : base(userService, cache, context, itemService)
        {
            _env = environment;
        }
        [HttpPost("AddNewItem")]
        [Authorize(Roles = "Admin,Maneger")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddNewItem([FromForm] ItemDto itemDto,[FromForm] IFormFile formFile)
        {
            try
            {
                if (string.IsNullOrEmpty(itemDto.ItemName) || string.IsNullOrEmpty(itemDto.Description) ||
                    string.IsNullOrEmpty(itemDto.Price.ToString()))
                {
                    return BadRequest("يرجى ادخال البيانات");
                }
                if (itemDto.CategoryId <= 0)
                {
                    return BadRequest("يرجى اختيار الفئة");
                }
                if (itemDto.ItemImage == null || itemDto.ItemImage.Length == 0)
                {
                    return BadRequest("يرجى ارفاق صورة للمنتج");
                }
                ItemEntity Item = await _itemService.GetItem(itemDto.ItemName);
                if (Item!=null)
                {
                    return BadRequest("المنتج موجود بالفعل");
                }
                if (formFile == null || formFile.Length == 0)
                    return BadRequest("File is empty");

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var ext = Path.GetExtension(formFile.FileName).ToLower();

                if (!allowedExtensions.Contains(ext))
                    return BadRequest("Invalid file type");

                var uploadsPath = Path.Combine(_env.WebRootPath, "uploads");

                if (!Directory.Exists(uploadsPath))
                    Directory.CreateDirectory(uploadsPath);

                var fileName = $"{Guid.NewGuid()}{ext}";
                var fullPath = Path.Combine(uploadsPath, fileName);

                using var stream = new FileStream(fullPath, FileMode.Open);
                await formFile.CopyToAsync(stream);

                itemDto.ItemImage = $"/uploads/{fileName}";
                Item = new ItemEntity
                {
                    ItemName = itemDto.ItemName,
                    Description = itemDto.Description,
                    CategoryId = itemDto.CategoryId,
                    Price = itemDto.Price,
                    Type = itemDto.Type ?? "Default",
                    Showing = itemDto.Shwoing,
                    ItemImage = itemDto.ItemImage, 
                    ImageContentType = itemDto.ImageContentType,
                    CreateUserId = UserManager.Id,
                    CreateDate = DateTime.Now.AddDays(3),
                };
                await _context.Items.AddAsync(Item);
                await _context.SaveChangesAsync();
                return Ok("تم اضافة المنتج بنجاح");

            }
            catch (Exception ex)
            {
                Loger.Write(ex, "ItemController => AddNewItem");
                return BadRequest("Error occurred while adding new item.");
            }
        }
        [HttpPost("UpdateItem")]
        [Authorize(Roles = "Admin,Maneger")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateItem([FromForm] ItemDto dto, [FromForm] IFormFile file)
        {
            try
            {
                ItemEntity item = await _itemService.GetItem(dto.Id);
                if(item == null)
                {
                    return BadRequest("المنتج غير موجود");
                }
                if (!string.IsNullOrEmpty(dto.ItemName))
                {
                    item.ItemName = dto.ItemName;
                }
                if (!string.IsNullOrEmpty(dto.Description))
                {
                    item.Description = dto.Description;
                }
                if (dto.CategoryId > 0)
                {
                    item.CategoryId = dto.CategoryId;
                }
                if (dto.Price > 0)
                {
                    item.Price = dto.Price;
                }
                if (dto.ItemImage != null && dto.ItemImage.Length > 0)
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                    var ext = Path.GetExtension(file.FileName).ToLower();

                    if (!allowedExtensions.Contains(ext))
                        return BadRequest("Invalid file type");

                    var uploadsPath = Path.Combine(_env.WebRootPath, "uploads");

                    if (!Directory.Exists(uploadsPath))
                        Directory.CreateDirectory(uploadsPath);

                    var fileName = $"{Guid.NewGuid()}{ext}";
                    var fullPath = Path.Combine(uploadsPath, fileName);

                    using var stream = new FileStream(fullPath, FileMode.OpenOrCreate);
                    await file.CopyToAsync(stream);

                    // هذا اللي ينخزن بالـ DB
                    item.ItemImage = $"/uploads/{fileName}";
                }
                item.UpdateUserId = UserManager.Id;
                item.Showing = dto.Shwoing;
                item.UpdateDate = DateTime.Now.AddDays(3);
                _context.Items.Update(item);
                await _context.SaveChangesAsync();
                return Ok("تم تحديث المنتج بنجاح");
            }
            catch (Exception ex)
            {
                Loger.Write(ex, "ItemController => UpdateItem");
                return BadRequest("Error occurred while updating item.");
            }
        }
        [HttpDelete("DeleteItem/{id}")]
        [Authorize (Roles = "Admin,Maneger")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            try
            {
                var item = await _itemService.GetItem(id);
                if (item == null)
                {
                    return NotFound("المنتج غير موجود");
                }
                _context.Items.Remove(item);
                await _context.SaveChangesAsync();
                return Ok("تم حذف المنتج بنجاح");
            }
            catch (Exception ex)
            {
                Loger.Write(ex, "ItemController => DeleteItem");
                return BadRequest("Error occurred while deleting item.");
            }
        }
        [HttpPost("GetAllItems")]
        public async Task<IActionResult> GetAllItems(PageDto pageDto)
        {
            try
            {
                var items = await _itemService.GetAllItems(pageDto.pageIndex,pageDto.pageSize);
                if (items == null || items.Count == 0)
                {
                    return NotFound("لا توجد منتجات");
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                Loger.Write(ex, "ItemController => GetAllItems");
                return BadRequest("Error occurred while retrieving items.");
            }
        }
        [HttpPost("GetItemByCategoryId/{categoryId}")]
        public async Task<IActionResult> GetItemByCategoryId(int categoryId,PageDto dto)
        {
            try
            {
                _categoryId = categoryId;
                List<ItemDto> Item = new List<ItemDto>();
                if(ItemManager.Count > 0)
                {
                    _items.Items = ItemManager;
                }
                if (ItemManager != null && ItemManager.Count > 0 && ItemManager.Count > dto.pageSize*(dto.pageIndex-1))
                {
                     Item = ItemManager;
                }
                else
                {
                    Item = await _itemService.GetAllItemsByCategoryId(categoryId,dto.pageIndex,dto.pageSize);
                    _items.Items.AddRange(Item);
                    ItemManager = _items.Items;
                }
                if (Item == null || Item.Count == 0)
                {
                    return NotFound("لا توجد منتجات في هذه الفئة");
                }
                
                return Ok(Item);
            }
            catch (Exception ex)
            {
                Loger.Write(ex, "ItemController => GetItemByCategoryId");
                return BadRequest("Error occurred while retrieving items by category.");
            }
        }
        [HttpGet("GetItem/{id}")]
        public async Task<IActionResult> GetItem(int id)
        {
            try
            {
                ItemDto item = await _itemService.GetItemDto(id);
                if (item == null)
                {
                    return NotFound("المنتج غير موجود");
                }
                return Ok(item);
            }
            catch (Exception ex)
            {
                Loger.Write(ex, "ItemController => GetItem");
                return BadRequest("Error occurred while retrieving item.");
            }
        }
        [HttpGet("GetItemByName/{Name}")]
        public async Task<IActionResult> GetItemByName(string Name)
        {
            try
            {
                List<ItemEntity> items = await _itemService.GetItemsByName(Name);
                if (items == null)
                {
                    return NotFound("المنتج غير موجود");
                }
                List<ItemDto> itemDto = items.Select(i => new ItemDto
                {
                    Id = i.Id,
                    ItemName = i.ItemName,
                    Description = i.Description,
                    CategoryId = i.CategoryId,
                    Price = i.Price,
                    ItemImage = i.ItemImage,
                    ImageContentType = i.ImageContentType
                }).ToList();
                return Ok(itemDto);
            }
            catch (Exception ex)
            {
                Loger.Write(ex, "ItemController => GetItem");
                return BadRequest("Error occurred while retrieving item.");
            }
        }
    }
}
