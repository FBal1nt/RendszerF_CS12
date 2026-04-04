namespace JegyMester.DataContext.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public ICollection<TicketPurchase> TicketPurchases { get; set; } = new List<TicketPurchase>();
        public ICollection<Role> Roles { get; set; } = new List<Role>();
    }
}