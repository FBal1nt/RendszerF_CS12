using Microsoft.AspNetCore.Identity;

namespace JegyMester.DataContext.Entities
{
    public class User : IdentityUser
    {
        public override string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public override string? Email { get; set; } = string.Empty;
        public override string? PhoneNumber { get; set; } = string.Empty;
        public ICollection<TicketPurchase> TicketPurchases { get; set; } = new List<TicketPurchase>();
        public ICollection<Role> Roles { get; set; } = new List<Role>();
    }
}