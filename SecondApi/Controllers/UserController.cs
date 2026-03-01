using App.ISirvice;
using Domin.User;
using Infrastructure.Cache;
using Infrastructure.ORM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using SecondApi.App.ISirvice;
using SherdProject.DTO;
using System.Threading.Tasks;

namespace SecondApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : MasterController
    {
        public UserController(IUserService userService, IAppMemoryCache cache, DBContext context,IItemService itemService)
            : base(userService, cache, context, itemService)
        {
        }
        [HttpGet("GetUserList")]
        [Authorize(Roles ="Maneger")]
        public async Task<IActionResult> GetUserListAsync(int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                List<User> users = await _usersService.GetUserList(pageIndex, pageSize);
                if (users == null || users.Count == 0)
                {
                    return NotFound("لا يوجد مستخدمين");
                }
                List<UserOut> userOuts = users.Select(u => new UserOut
                {
                    Id = u.Id,
                    Name = u.Username,
                    Role = u.Role
                }).ToList();
                return Ok(userOuts);
            }
            catch (Exception ex)
            {
                Loger.Write(ex, "UserController => GetUserListAsync");
                return BadRequest("حدث خطا ما");
            }
        }
        [HttpGet("GetUser/{id}")]
        [Authorize(Roles = "Maneger")]
        public  IActionResult GetUser(int id)
        {
            try
            {
                User user = _usersService.GetUser(id);
                if (user == null)
                {
                    return NotFound("User not found");
                }
                UserOut userOut=new UserOut
                {
                    Id = user.Id,
                    Name = user.Username,
                    Role = user.Role
                };
                
                return Ok(user);
            }
            catch (Exception ex)
            {
                Loger.Write(ex, "UserController => GetUser");
                return BadRequest("An error occurred while retrieving the user");
            }
        }
        [HttpGet("GetUserByName/{Name}")]
        [Authorize(Roles = "Maneger")]
        public async Task<IActionResult> GetUserByName(string Name)
        {
            try
            {
                List<User> users = await _usersService.GetUserByName(Name);
                if (users == null)
                {
                    return NotFound("Users not found");
                }
                List<UserOut> userOuts = users.Select(u => new UserOut
                {
                    Id = u.Id,
                    Name = u.Username,
                    Role = u.Role
                }).ToList();
                return Ok(userOuts);
            }
            catch (Exception ex)
            {
                Loger.Write(ex, "UserController => GetUser");
                return BadRequest("An error occurred while retrieving the user");
            }
        }
        [HttpGet("GetMyInfo")]
        [Authorize]
        public IActionResult GetMyInfo()
        {
            try
            {
                UserOut userOut = new UserOut()
                {
                    Id = UserManager.Id,
                    Name = UserManager.Username,
                    Role = UserManager.Role
                };
                return Ok(userOut);
            }
            catch (Exception ex)
            {
                Loger.Write(ex, "UserController => ChangeUserRole");
                return BadRequest("An error occurred while changing the user role");
            }
        }
        [HttpGet("MakeUserAdmin/{id}")]
        [Authorize(Roles = "Maneger")]
        public async Task<IActionResult> MakeUserAdmin(int id)
        {
            try
            {
                User user = _usersService.GetUser(id);
                if (user == null)
                {
                    return NotFound("User not found");
                }
                if (user.Role == "Admin")
                {
                    return BadRequest("User is already an Admin");
                }
                user.Role = "Admin";
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return Ok("تمت العملية بنجاح");
            }
            catch (Exception ex)
            {
                Loger.Write(ex, "UserController => ChangeUserRole");
                return BadRequest("An error occurred while changing the user role");
            }
        }
        [HttpGet("MakeAdminUser/{id}")]
        [Authorize(Roles = "Maneger")]
        public async Task<IActionResult> MakeAdminUser(int id)
        {
            try
            {
                User user = _usersService.GetUser(id);
                if (user == null)
                {
                    return NotFound("User not found");
                }
                if (user.Role != "Admin")
                {
                    return BadRequest("User is not an admin");
                }
                user.Role = "User";
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return Ok("تمت العملية بنجاح");
            }
            catch (Exception ex)
            {
                Loger.Write(ex, "UserController => ChangeUserRole");
                return BadRequest("An error occurred while changing the user role");
            }
        }
    }
}
