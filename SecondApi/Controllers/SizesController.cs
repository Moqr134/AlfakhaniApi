using App.ISirvice;
using Infrastructure.Cache;
using Infrastructure.ORM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecondApi.App.ISirvice;
using SecondApi.Domin.Sizes;
using SherdProject.DTO;

namespace SecondApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SizesController : MasterController
    {
        private ISizesService _sizesService;
        public SizesController(IUserService userService, IAppMemoryCache cache, DBContext context, ISizesService service,IItemService itemService) : base(userService, cache, context,itemService)
        {
            _sizesService = service;
        }
        [HttpPost("AddNewSize")]
        [Authorize(Roles = "Admin,Maneger")]
        public async Task<IActionResult> AddNewSize(SizeDto dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest("البيانات غير صحيحة");
                }
                if (string.IsNullOrEmpty(dto.Name))
                {
                    return BadRequest("الاسم مطلوب");
                }
                if (dto.ItemId <= 0)
                {
                    return BadRequest("المعرف غير صحيح");
                }
                else if (dto.Price <= 0)
                {
                    return BadRequest("السعر غير صحيح");
                }
                SizesEntity sizesEntity = await _sizesService.GetSize(dto.Name,dto.ItemId);
                if (sizesEntity != null)
                {
                    return NotFound("هذا الحجم موجود بالفعل");
                }
                bool IsTrue = await _sizesService.AddSize(dto);
                if (IsTrue)
                {
                    return Ok("تمت الاضافة بنجاح");
                }
                else
                {
                    return BadRequest("حدث خطأ أثناء الإضافة");
                }
            }
            catch (Exception ex)
            {
                await Loger.WriteAsync(ex, "SizesController => AddNewSize");
                return BadRequest("حدث خطا ما");
            }
        }
        [HttpDelete("DeleteSize")]
        [Authorize (Roles = "Admin,Maneger")]
        public async Task<IActionResult> DeleteSize(SizeDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.Name))
                {
                    return BadRequest("المعرف غير صحيح");
                }
                if(dto.ItemId <= 0)
                {
                    return BadRequest("المعرف غير صحيح");
                }
                string IsTrue = await _sizesService.DeleteSize(dto);
                if (IsTrue=="true")
                {
                    return Ok("تم الحذف بنجاح");
                }
                else
                {
                    return BadRequest(IsTrue);
                }
            }
            catch (Exception ex)
            {
                await Loger.WriteAsync(ex, "SizesController => DeleteSize");
                return BadRequest("حدث خطا ما");
            }
        }
    }
}
