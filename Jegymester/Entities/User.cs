using Jegymester.Entities;
using System;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public IEnumerable<TicketPurchase> TicketPurchases { get; set; } = new List<TicketPurchase>();
    public IEnumerable<Role> Roles { get; set; } = new List<Role>();
    public IEnumerable<Screening> Screenings { get; set; } = new List<Screening>();
}
