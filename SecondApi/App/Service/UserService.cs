using App.ISirvice;
using Domin.User;
using Infrastructure.Logger;
using Infrastructure.ORM;
using Microsoft.EntityFrameworkCore;

namespace App.Service;

public class UserService : IUserService
{
    private DBContext Context;
    private Loger loger = new Loger();
    public UserService(DBContext dBContext)
    {
        Context = dBContext;
    }
    public async Task<User> CheckUser(string Eamil)
    {
        try
        {
           return await Context.Users.Where(x => x.Email == Eamil && x.IsRemoved == false && x.IsConfirm == true).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            await loger.WriteAsync(ex, "UserService => CheckUser");
            throw;
        }
    }

    public async Task<User> CheckUserExsist(string Name, string Email)
    {
        try
        {
            return await Context.Users.Where(x => x.Email == Email || x.Username == Name).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            await loger.WriteAsync(ex, "UserService => CheckUserExsist");
            throw;
        }
    }

    public async Task<User> ConfirmAcount(string Code, string Email, string Name)
    {
        try
        {
            return await Context.Users.Where(x => x.Email == Email && x.Username == Name && x.Code == Code).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            await loger.WriteAsync(ex, "UserService => ConfirmAcount");
            throw;
        }
    }

    public User GetUser(int id)
    {
        try
        {
            return Context.Users.Find(id);
        }
        catch (Exception ex)
        {
            loger.Write(ex, "UserService => GetUser");
            throw;
        }
    }
}
