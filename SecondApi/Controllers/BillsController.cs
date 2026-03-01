using App.ISirvice;
using Domin.User;
using Infrastructure.Cache;
using Infrastructure.ORM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecondApi.App.ISirvice;
using SecondApi.App.Service;
using SecondApi.Domin.Bills;
using SecondApi.Domin.Notifecation;
using SecondApi.Domin.User;
using SecondApi.Infrastructure.Notifications;
using SherdProject.DTO;

namespace SecondApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillsController : MasterController
    {
        private IBillsService BillsService;
        private INotificationService _notificationService;
        private IFcmService _fcmService;
        public BillsController(IFcmService fcmService,INotificationService notificationService,IUserService userService, IAppMemoryCache cache,IItemService itemService, DBContext context,BillsService service)
            : base(userService, cache, context, itemService)
        {
            _fcmService = fcmService;
            BillsService = service;
            _notificationService = notificationService;
        }
        [HttpPost("AddBills")]
        public async Task<IActionResult> AddBills(BillsDto bills)
        {
            try
            {
                if (bills == null || bills.BillsItems == null || !bills.BillsItems.Any())
                {
                    return NotFound("لا توجد عناصر");
                }
                if(DateTime.UtcNow.AddHours(3).Hour < 9)
                {
                    return BadRequest("عذرا المحل مغلق حاليا. يمكنكم الطلب بعد الساعة 9 صباحا وقبل الساعة 12 مسائاً");
                }
                if(string.IsNullOrEmpty(bills.Id))
                    bills.Id = Guid.NewGuid().ToString();
                Bills newBill = new Bills
                {
                    BillNumper=Guid.NewGuid(),
                    DviceToken = bills.Id,
                    TotalAmount = bills.TotalAmount,
                    Description = bills.Description,
                    Status = "Waiting",
                    CreateUserId=0,
                    CreateDate = DateTime.UtcNow.AddHours(3),
                    BillLocation=bills.Location,
                    BillName=bills.Name,
                    BillPhoneNumper=bills.PhoneNamper
                };
                List<BillsItem> billItems = new List<BillsItem>();
                foreach (var item in bills.BillsItems)
                {
                    if (item.TotalPrice <= 0 || string.IsNullOrEmpty(item.ItemName))
                    {
                        return BadRequest("العناصر غير صالحة");
                    }
                    billItems.Add(new BillsItem
                    {
                        ItemName = item.ItemName,
                        Price = item.Price,
                        Quantity = item.Quantity,
                        TotalPrice = item.TotalPrice,
                        ItemId = item.ItemId,
                        BillNumper = newBill.BillNumper
                    });
                }
                Response.Cookies.Append("FcmToken", bills.Id, new CookieOptions
                {
                    Path = "/",
                    //Domain = ".alfakhani.com",
                    HttpOnly = true,
                    SameSite = SameSiteMode.None,
                    Secure = true,
                    Expires = DateTime.UtcNow.AddHours(1)
                });
                _context.Bills.Add(newBill);
                _context.BillsItems.AddRange(billItems);
                await _context.SaveChangesAsync();
                List<UserConnection> users = await _usersService.GetAdminUsers();
                List<string>? conn = users.Where(users => !string.IsNullOrEmpty(users.ConnectionId)).Select(u => u.ConnectionId).ToList();
                List<string>? deviceTokens = users.Where(u => !string.IsNullOrEmpty(u.DeviceToken)).Select(u => u.DeviceToken).ToList();
                await _notificationService.SendNotificationAsync(conn, "لديك طلب جديد");
                bool isSend = await _fcmService.SendNotificationToGroup(deviceTokens);
                return Ok("تمت ارسال الطلب");
            }
            catch (Exception ex)
            {
                await Loger.WriteAsync(ex, "BillsController => AddBills");
                return BadRequest("حدث خطا ما");
            }
        }
        [HttpGet("GetBills")]
        [Authorize(Roles = "Admin,Maneger")]
        public async Task<IActionResult> GetBills()
        {
            try
            {
                List<Bills> bills = await BillsService.GetBillsAsync();
                if (bills == null || !bills.Any())
                {
                    return NotFound("لا توجد فواتير");
                }
                List<BillsAdminOut> billsOut = bills.Select(b => new BillsAdminOut
                {
                    Id = b.Id,
                    Date = b.CreateDate,
                    PhoneNamper= b.BillPhoneNumper,
                    Name=b.BillName,
                    Location=b.BillLocation,
                    TotalAmount = b.TotalAmount,
                    BillNumper=b.DviceToken,
                    Status = b.Status,
                    Description = b.Description,
                    BillsItems = b.billsItems.Select(bi => new BillsItemDto
                    {
                        ItemName = bi.ItemName,
                        Price = bi.Price,
                        Quantity = bi.Quantity,
                        TotalPrice = bi.TotalPrice,
                        ItemId = bi.ItemId
                    }).ToList()
                }).ToList();
                return Ok(billsOut);
            }catch(Exception ex)
            {
                await Loger.WriteAsync(ex, "BillsController => GetBills");
                return BadRequest("حدث خطا ما");
            }
        }
        [HttpGet("GetBillsById/{id}")]
        [Authorize("Admin,Maneger")]
        public async Task<IActionResult> GetBillsById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("معرف الفاتورة غير صالح");
                }
                Bills? bill = await BillsService.GetBillByIdAsync(id);
                if (bill == null)
                {
                    return NotFound("الفاتورة غير موجودة");
                }
                BillsOut billOut = new BillsOut
                {
                    Id = bill.Id,
                    Date = bill.CreateDate,
                    TotalAmount = bill.TotalAmount,
                    Status = bill.Status,
                    BillsItems = bill.billsItems?.Select(bi => new BillsItemDto
                    {
                        ItemName = bi.ItemName,
                        Price = bi.Price,
                        Quantity = bi.Quantity,
                        TotalPrice = bi.TotalPrice,
                        ItemId = bi.ItemId
                    }).ToList()
                };
                return Ok(billOut);
            }
            catch(Exception ex)
            {
                await Loger.WriteAsync(ex, "BillsController => GetBillsById");
                return BadRequest("حدث خطا ما");
            }
        }
        [HttpPost("ChangeStutes")]
        [Authorize(Roles = "Admin,Maneger")]
        public async Task<IActionResult> ChangeStutes(ChangeStutesDto changeStutes)
        {
            try
            {
                if (changeStutes == null || string.IsNullOrEmpty(changeStutes.Numper) || string.IsNullOrEmpty(changeStutes.Status))
                {
                    return BadRequest("بيانات غير صالحة");
                }
                Bills? bill = await BillsService.GetBillByIdAsync(changeStutes.Numper);
                if (bill == null)
                {
                    return NotFound("الفاتورة غير موجودة");
                }
                bill.Status = changeStutes.Status;
                if (changeStutes.Status == "Cancelled" && !string.IsNullOrEmpty(changeStutes.RefuseReason))
                {
                    bill.RefuseReason = changeStutes.RefuseReason;
                }
                else if (changeStutes.Status != "Cancelled")
                {
                    bill.RefuseReason = string.Empty;
                }
                bill.UpdateDate = DateTime.UtcNow.AddHours(3);
                if(_UserId==0)
                    GetUserId();
                bill.UpdateUserId = _UserId;
                _context.Bills.Entry(bill);
                await _context.SaveChangesAsync();
                MessageRequst messageRequst = new MessageRequst
                {
                    Body = "تغيرت حالة طلبك اذهب الى مركز الاشعارات لرؤية التفاصيل",
                    Title = "تغيرت حالة طلبك",
                    DeviceToken = bill.DviceToken,
                };
                if (bill.Status == "Cancelled")
                {
                    messageRequst.Body = $"تم رفض طلبك. السبب: {bill.RefuseReason}";
                }
                else if (bill.Status == "InProgress")
                {
                    messageRequst.Body = "طلبك قيد التجهيز.";
                }
                else if (bill.Status == "Completed")
                {
                    messageRequst.Body = "الطلب في طريقه اليك";
                }else if (bill.Status =="Done")
                {
                    messageRequst.Body = "تم توصيل الطلب بنجاح. شكراً لاستخدامك خدمتنا!";
                }
                bool IsGuid = Guid.TryParse(bill.DviceToken, out Guid deviceGuid);
                if (IsGuid)
                {
                    return Ok("تم تغيير الحالة بنجاح ولكن لم يتم ارسال اشعار الى المستخدم");
                }
                bool isSend = await _fcmService.SendNotificationAsync(messageRequst); 
                if (isSend) 
                    return Ok("تم تغيير الحالة بنجاح");
                else
                    return Ok("تم تغيير الحالة بنجاح ولكن لم يتم ارسال اشعار الى المستخدم");
            }
            catch(Exception ex)
            {
                await Loger.WriteAsync(ex, "BillsController => ChangeStutes");
                return BadRequest("حدث خطا ما");
            }
        }
        [HttpGet("GetMyBills")]
        public async Task<IActionResult> GetMyBills()
        {
            try
            {
                string ? fcmToken = Request.Cookies["FcmToken"];
                if (string.IsNullOrEmpty(fcmToken))
                {
                    return BadRequest("رمز الجهاز غير صالح");
                }
                List<Bills> bills = await BillsService.GetAllMyBills(fcmToken);
                if (bills == null || !bills.Any())
                {
                    return NotFound();
                }
                List<BillsOut> billsOut = bills.Select(b => new BillsOut
                {
                    Id = b.Id,
                    Date = b.CreateDate,
                    TotalAmount = b.TotalAmount,
                    Status = b.Status,
                    BillsItems = b.billsItems?.Select(bi => new BillsItemDto
                    {
                        ItemName = bi.ItemName,
                        Price = bi.Price,
                        Quantity = bi.Quantity,
                        TotalPrice = bi.TotalPrice,
                        ItemId = bi.ItemId
                    }).ToList()
                }).ToList();
                return Ok(billsOut);
            }
            catch(Exception ex)
            {
                await Loger.WriteAsync(ex, "BillsController => GetMyBills");
                return BadRequest("حدث خطا ما");
            }
        }
        [HttpPost("GetManagerBills")]
        [Authorize (Roles = "Maneger")]
        public async Task<IActionResult> GetManagerBills(PageDto pageDto)
        {
            try
            {
                List<BillManegearOut> bills = await BillsService.GetBills(pageDto.pageIndex, pageDto.pageSize);
                if (bills == null || !bills.Any())
                {
                    return NotFound("لا توجد فواتير");
                }
                return Ok(bills);
            }
            catch(Exception ex)
            {
                await Loger.WriteAsync(ex, "BillsController => GetManagerBills");
                return BadRequest("حدث خطا ما");
            }
        }
        [HttpPost("CancleOrder")]
        public async Task<IActionResult> CancelOrder(BillsOut billOut)
        {
            try
            {
                if (billOut == null || billOut.Id <= 0)
                {
                    return BadRequest("بيانات غير صالحة");
                }
                Bills? bill = await BillsService.GetBillByIdAsync(billOut.Id);
                if (bill == null)
                {
                    return NotFound("الفاتورة غير موجودة");
                }
                bill.Status = "Cancelled";
                bill.RemoveDate = DateTime.UtcNow.AddHours(3);
                bill.RefuseReason = "تم إلغاء الطلب من قبل المستخدم";
                _context.Bills.Entry(bill);
                await _context.SaveChangesAsync();
                return Ok("تم إلغاء الطلب بنجاح");
            }
            catch(Exception ex)
            {
                await Loger.WriteAsync(ex, "BillsController => CancelOrder");
                return BadRequest("حدث خطا ما");
            }
        }
    }
}
