using App.ISirvice;
using FirebaseAdmin.Messaging;
using Infrastructure.Cache;
using Infrastructure.ORM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecondApi.App.ISirvice;
using SecondApi.Domin.Notifecation;
using SecondApi.Domin.User;
using SherdProject.DTO;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SecondApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotifController : MasterController
    {
        INotifService _notifService;
        public NotifController(IUserService userService,IItemService itemService, IAppMemoryCache cache, DBContext context, INotifService notifService)
            : base(userService, cache, context, itemService)
        {
            _notifService = notifService;
        }
        [HttpGet("GetNotif")]
        [Authorize]
        public async Task<IActionResult> GetNotif()
        {
            try
            {
                List<Notifecation> notifecations = await _notifService.GetAll(UserManager.Id);
                if (notifecations == null || notifecations.Count == 0)
                {
                    return NotFound();
                }
                List<NotifecationDto> Dto = new List<NotifecationDto>();
                foreach (var Noti in notifecations)
                {
                    Dto.Add(new NotifecationDto
                    {
                        Id = Noti.Id,
                        Title = Noti.Title,
                        Stutes = Noti.Stutes,
                        CreatedAt = Noti.CreatedAt,
                        RefuseReason = Noti.RefuseReason
                    });
                }
                return Ok(Dto);
            }
            catch (Exception ex)
            {
                await Loger.WriteAsync(ex, "NotifController => GetNotif");
                return BadRequest("حدث خطا اثناء جلب الاشعارات");
            }
        }
        [HttpDelete("DeleteNotif/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteNotif(int id)
        {
            try
            {
                Notifecation? notifecation = await _context.Notifecations.FindAsync(id);
                if (notifecation == null)
                {
                    return NotFound("لا يوجد اشعار بهذا المعرف");
                }
                _context.Notifecations.Remove(notifecation);
                await _context.SaveChangesAsync();
                return Ok("تم حذف الاشعار بنجاح");
            }
            catch (Exception ex)
            {
                await Loger.WriteAsync(ex, "NotifController => DeleteNotif");
                return BadRequest("حدث خطا اثناء حذف الاشعار");
            }
        }
    }
}
