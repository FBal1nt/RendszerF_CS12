namespace JegyMester.DataContext.Entities
{
    public class TicketPurchase
    {
        public int Id { get; set; }
        public DateTime PurchaseDateTime { get; set; }
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
        public decimal TotalPrice => Tickets?.Sum(t => t.Price) ?? 0;
        public int UserId { get; set; }
        public User? User { get; set; }
    }
}