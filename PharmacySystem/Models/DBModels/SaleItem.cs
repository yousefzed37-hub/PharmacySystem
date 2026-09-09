namespace PharmacySystem.Models.DBModels
{
    public class SaleItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal { get; set; }
        public int SaleId { get; set; }
        public Sale? Sale { get; set; }
        public int MedicineId { get; set; }
        public Medicine? Medicine { get; set; }
    }
}
