using App.ISirvice;
using Domin.User;
using Infrastructure.Cache;
using Infrastructure.JWT;
using Infrastructure.Logger;
using Infrastructure.ORM;
using Microsoft.AspNetCore.Mvc;

namespace SecondApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MasterController : ControllerBase
    {

        public readonly Loger Loger;
        public DBContext _context;
        private IAppMemoryCache _cache;
        public int _UserId = 0;
        private readonly Jwt jwt;
        public IUserService _usersService;
        public MasterController(IUserService userService, IAppMemoryCache cache, DBContext context)
        {
            _context = context;
            _cache = cache;
            jwt = new Jwt();
            Loger = new Loger();
            _usersService = userService;
        }
        public User UserManager
        {
            get
            {
                if (_UserId == 0)
                    GetUserId();
                if (_cache.IsExist("User" + _UserId))
                    return _cache.Get<User>("User" + _UserId);
                else
                {
                    return ResetUserinfo();
                }
            }
            set
            {
                _cache.Set("User" + _UserId, value);
            }
        }
        private User ResetUserinfo()
        {
            try
            {
                if (_UserId == 0)
                    GetUserId();
                UserManager = _usersService.GetUser(_UserId);
                return UserManager;
            }
            catch (Exception ex)
            {
                new Loger().Write(ex, "MasterController => ResetUserinfo");
                throw;
            }
        }
        protected void GetUserId()
        {
            _UserId = jwt.ValidateToken(HttpContext.Request.Headers["Authorization"].ToString());
        }
    }
}
