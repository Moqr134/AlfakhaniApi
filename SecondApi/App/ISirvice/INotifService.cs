using SecondApi.Domin.Notifecation;

namespace SecondApi.App.ISirvice
{
    public interface INotifService
    {
        public Task<List<Notifecation>> GetAll(int userId);
    }
}
