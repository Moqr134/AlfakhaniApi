using SecondApi.Domin.Bills;
using SherdProject.DTO;

namespace SecondApi.App.ISirvice
{
    public interface IBillsService
    {
        public Task<List<Bills>> GetBillsAsync();
        public Task<Bills> GetBillByIdAsync(string id);
        public Task<Bills> GetBillByIdAsync(int id);
        public Task<List<Bills>> GetAllMyBills(string id);
        public Task<List<BillManegearOut>> GetBills(int pageIndex = 1, int pageSize = 10);

    }
}
