using Infrastructure.ORM;
using Infrastructure.Service;
using Microsoft.EntityFrameworkCore;
using SecondApi.App.ISirvice;
using SecondApi.Domin.Bills;
using SecondApi.Infrastructure.Services;
using SherdProject.DTO;

namespace SecondApi.App.Service
{
    public class BillsService : MasterService,IBillsService,IScopped
    {
        public BillsService(DBContext context): base(context)
        {
        }

        public async Task<List<Bills>> GetAllMyBills(string id)
        {
            try
            {
                return await Context.Bills
                    .Include(b => b.billsItems)
                    .Where(b => b.DviceToken == id && b.IsRemoved == false && b.Status!="Done"&& b.Status!= "Cancelled")
                    .OrderByDescending(b => b.Id)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                await loger.WriteAsync(ex, "BillsService => GetAllMyBills");
                throw;
            }
        }

        public async Task<Bills> GetBillByIdAsync(string id)
        {
            try
            {
               return await Context.Bills
                    .Include(b => b.billsItems)
                    .FirstOrDefaultAsync(b => b.DviceToken == id && b.IsRemoved == false);
            }
            catch (Exception ex)
            {
                await loger.WriteAsync(ex, "BillsService => GetBillByIdAsync");
                throw;
            }
        }

        public async Task<Bills> GetBillByIdAsync(int id)
        {
            try
            {
                return await Context.Bills
                     .Include(b => b.billsItems)
                     .FirstOrDefaultAsync(b => b.Id == id && b.IsRemoved == false);
            }
            catch (Exception ex)
            {
                await loger.WriteAsync(ex, "BillsService => GetBillByIdAsync");
                throw;
            }
        }

        public async Task<List<BillManegearOut>> GetBills(int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                return await Context.Bills
                    .Where(b => b.IsRemoved == false && b.Status == "Cancelled" || b.Status == "Done")
                    .OrderByDescending(b => b.Id)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .Join(Context.Users,
                        b => b.UpdateUserId,
                        u => u.Id,
                        (b, u) => new BillManegearOut
                        {
                            Id = b.Id,
                            BillNumper = b.DviceToken,
                            Date = b.CreateDate,
                            UpdatedBy = u.Username,
                            Name = b.BillName,
                            Status = b.Status,
                            RefuseReason = b.RefuseReason,
                            TotalAmount = b.TotalAmount
                        })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                await loger.WriteAsync(ex, "BillsService => GetBills");
                throw;
            }
        }

        public async Task<List<Bills>> GetBillsAsync()
        {
            try
            {
                return await Context.Bills
                     .Include(b => b.billsItems)
                     .Where(b => b.IsRemoved == false&&b.Status!= "Cancelled" && b.Status!= "Done")
                     .OrderByDescending(b => b.Id)
                     .ToListAsync();
            }
            catch (Exception ex)
            {
                await loger.WriteAsync(ex, "BillsService => GetBillByIdAsync");
                throw;
            }
        }
    }
}
