using App.ISirvice;
using Domin.User;
using Infrastructure.Cache;
using Infrastructure.JWT;
using Infrastructure.ORM;
using Infrastructure.PassowdHashing;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SecondApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : MasterController
    {
        PasswordHashing passwordHashing;
        MailService mailService;
        public AccountController(IUserService userService, IAppMemoryCache cache, DBContext context) : base(userService, cache, context)
        {
            passwordHashing = new PasswordHashing();
            mailService = new MailService();
        }
        [HttpPost]
        
        public async Task<IActionResult> Login(string Email,string Password)
        {
            try
            {
                User user = await _usersService.CheckUser(Email);
                if (user == null)
                    return NotFound("عذرا المستخدم غير موجود");
                bool Check = passwordHashing.VerifyPassword(Password, user.HashPassword);
                if (!Check)
                    return NotFound("عذرا كلمه المرور غير صحيحة");
                user.IsOnline = true;
                user.LastLogin = DateTime.UtcNow.AddHours(3);
                user.Token = new Jwt().GenerateToken(user.Id, user.Username, user.IsAdmin);
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                _UserId = user.Id;
                UserManager = user;
                return Ok(user.Token);
            }
            catch (Exception ex)
            {
                await Loger.WriteAsync(ex, "AccountController => Login");
                return NotFound("حدث خطا ما");
            }
        }
        [HttpPost]
        
        public async Task<IActionResult> Register(string name, string password, string email)
        {
            try
            {
                User check = await _usersService.CheckUserExsist(name, email);
                if (check != null)
                    return NotFound();
                Random random = new Random();
                string PasswordHasher = passwordHashing.HashPassword(password);
                User users = new User
                {
                    Username = name,
                    HashPassword = PasswordHasher,
                    Email = email,
                    IsActive = true,
                    IsRemoved = false,
                    IsOnline = false,
                    IsConfirm = false,
                    Code = random.Next(100000, 999999).ToString()
                };
                mailService.SendMail(email, "Confirm Account", "Code:" + users.Code);
                await _context.Users.AddAsync(users);
                await _context.SaveChangesAsync();
                return Ok("تم انشاء الحساب بنجاح يرجى مراجعه البريد الألكتروني لتأكيده");
            }
            catch (Exception ex)
            {
                await Loger.WriteAsync(ex, "AccountController => Register");
                return NotFound();
            }
        }
    }
}
