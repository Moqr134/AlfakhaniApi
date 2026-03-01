using Common.App;

namespace SecondApi.Domin.Bills
{
    public class BillsItem
    {
        public int Id { get; set; }
        public Guid? BillNumper { get; set; }
        public required string ItemName { get; set; }
        public required int ItemId { get; set; }
        public required double Quantity { get; set; } = 1;
        public required double Price { get; set; }
        public required double TotalPrice { get; set; }
        public Bills? Bills { get; set; }
    }
}
