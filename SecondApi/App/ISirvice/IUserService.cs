using Domin.User;

namespace App.ISirvice;

public interface IUserService
{
    public User GetUser(int id);
    public Task<User> CheckUser(string Eamil);
    public Task<User> CheckUserExsist(string Name, string Email);
    public Task<User> ConfirmAcount(string Code, string Email, string Name);
}
