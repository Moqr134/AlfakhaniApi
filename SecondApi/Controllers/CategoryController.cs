using App.ISirvice;
using Infrastructure.Cache;
using Infrastructure.ORM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecondApi.App.ISirvice;
using SecondApi.Domin.Category;
using SherdProject.DTO;
using System.Threading.Tasks;

namespace SecondApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : MasterController
    {
        private ICategoryService _categoryService;
        public CategoryController(IUserService userService, IAppMemoryCache cache, DBContext context,ICategoryService categoryService,IItemService itemService)
            : base(userService, cache, context,itemService)
        {
            _categoryService = categoryService;
        }
        [HttpPost("GetAllCategories")]
        [Authorize]
        public async Task<IActionResult> GetAllCategories(PageDto pageDto)
        {
            try
            {
                var categories = await _categoryService.GetAllCategories(pageDto.pageIndex,pageDto.pageSize);
                if (categories == null || !categories.Any())
                {
                    return NotFound("لا يوجد فئات لعرضها");
                }
                List<CategoryDto> dto = new List<CategoryDto>();
                foreach (var category in categories)
                {
                    dto.Add(new CategoryDto
                    {
                        Id = category.Id,
                        CategoryName = category.CategoryName,
                        Items = category.Items?.Select(i => new ItemDto
                        {
                            Id = i.Id,
                            ItemName = i.ItemName,
                            Description = i.Description,
                            Price = i.Price,
                            Type = i.Type,
                            ItemImage = i.ItemImage,
                            ImageContentType = i.ImageContentType,
                            Sizes = i.Sizes?.Select(s => new SizeDto
                            {
                                Id = s.Id,
                                Name = s.Name,
                                ItemId = s.ItemId,
                                Price = s.Price
                            }).ToList()
                        }).ToList()
                    });
                }
                return Ok(dto);
            }
            catch (Exception ex)
            {
                await Loger.WriteAsync(ex, "CategoryController => GetAllCategories");
                return BadRequest("Error occurred while geting all categories");
            }
        }
        [HttpGet("GetCategories")]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var categories = await _categoryService.GetCategories();
                if (categories == null || !categories.Any())
                {
                    return NotFound("لا يوجد فئات لعرضها");
                }
                return Ok(categories);
            }
            catch (Exception ex)
            {
                await Loger.WriteAsync(ex, "CategoryController => GetCategories");
                return BadRequest("Error occurred while getting categories");
            }
        }
        [HttpGet("GetCategory/{id}")]
        [Authorize (Roles = "Admin,Maneger")]
        public async Task<IActionResult> GetCategory(int id)
        {
            try
            {
                var category = await _categoryService.GetCategory(id);
                if (category == null)
                {
                    return NotFound("الفئة غير موجودة");
                }
                CategoryDto dto = new CategoryDto
                {
                    Id = category.Id,
                    CategoryName = category.CategoryName,
                };
                return Ok(dto);
            }
            catch (Exception ex)
            {
                await Loger.WriteAsync(ex, "CategoryController => GetCategory");
                return BadRequest("Error occurred while getting category");
            }
        }
        [HttpPost("AddCategory")]
        [Authorize (Roles = "Admin,Maneger")]
        public async Task<IActionResult> AddCategory(CategoryDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.CategoryName))
                {
                    return BadRequest("يرجى ادخال اسم الفئة");
                }
                CategoryEntity entity = await _categoryService.GetCategory(dto.CategoryName);
                if (entity is not null)
                {
                    return NotFound("هذه الفئة موجودة بلفعل");
                }
                CategoryEntity category = new CategoryEntity
                {
                    CategoryName = dto.CategoryName,
                    CreateDate = DateTime.Now.AddHours(3),
                    CreateUserId = UserManager.Id
                };
                await _context.Categories.AddAsync(category);
                await _context.SaveChangesAsync();
                return Ok("تم اضافة الفئة بنجاح");
            }
            catch (Exception ex)
            {
                await Loger.WriteAsync(ex, "CategoryController => AddCategory");
                return BadRequest("Error occurred while adding new category.");
            }
        }
        [HttpPost("UpdateCategory")]
        [Authorize(Roles = "Admin,Maneger")]
        public async Task<IActionResult> UpdateCategory(CategoryDto dto)
        {
            try
            {
                CategoryEntity category = await _categoryService.GetCategory(dto.Id);
                if (category == null)
                {
                    return BadRequest("الفئة غير موجودة");
                }
                if (string.IsNullOrEmpty(dto.CategoryName))
                {
                    return BadRequest("يرجى ادخال اسم الفئة");
                }
                category.CategoryName = dto.CategoryName;
                category.UpdateDate = DateTime.Now.AddHours(3);
                category.UpdateUserId = UserManager.Id;
                _context.Categories.Update(category);
                await _context.SaveChangesAsync();
                return Ok("تم تحديث الفئة بنجاح");
            }
            catch (Exception ex)
            {
                await Loger.WriteAsync(ex, "CategoryController => UpdateCategory");
                return BadRequest("Error occurred while updating category.");
            }
        }
        [HttpDelete("DeleteCategory/{id}")]
        [Authorize (Roles = "Admin,Maneger")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var category = await _categoryService.GetCategory(id);
                if (category == null)
                {
                    return NotFound("الفئة غير موجودة");
                }
                category.IsRemoved = true;
                category.RemoveDate = DateTime.Now.AddDays(3);
                category.RemoveUserId = UserManager.Id;
                _context.Categories.Update(category);
                await _context.SaveChangesAsync();
                return Ok("تم حذف الفئة بنجاح");
            }
            catch (Exception ex)
            {
                await Loger.WriteAsync(ex, "CategoryController => DeleteCategory");
                return BadRequest("Error occurred while deleting category.");
            }
        }
    }
}
