using Common.App;
using System.Numerics;

namespace SecondApi.Domin.Bills
{
    public class Bills:Entity
    {
        public required Guid BillNumper { get; set; }
        public required string DviceToken { get; set; }
        public required double TotalAmount { get; set; }
        public required string BillName { get; set; }
        public required string BillPhoneNumper { get; set; }
        public required string BillLocation {  get; set; }
        public string? Description { get; set; }
        public string? RefuseReason { get; set; } = string.Empty;
        public required string Status { get; set; } = "Waiting";
        public ICollection<BillsItem>? billsItems { get; set; }
    }
}
