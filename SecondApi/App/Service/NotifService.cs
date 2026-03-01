using Infrastructure.ORM;
using Microsoft.EntityFrameworkCore;
using SecondApi.App.ISirvice;
using SecondApi.Domin.Notifecation;
using SecondApi.Infrastructure.Services;

namespace SecondApi.App.Service
{
    public class NotifService : MasterService,INotifService
    {
        public NotifService(DBContext context):base(context)
        {
        }
        public async Task<List<Notifecation>> GetAll(int userId)
        {
            try
            {
                return await Context.Notifecations.Where(x => x.UserId == userId)
                    .OrderByDescending(x => x.Id)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                await loger.WriteAsync(ex, "ItemService => GetAllItems");
                throw;
            }
        }
    }
}
