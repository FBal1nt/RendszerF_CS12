using JegyMester.DataContext.Enums;
using Microsoft.EntityFrameworkCore;

namespace JegyMester.DataContext.Entities
{
    [Index(nameof(ScreeningId), nameof(Row), nameof(SeatNumber), IsUnique = true)]
    public class Ticket
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public TicketType Type { get; set; }
        public bool Valid { get; set; }
        public decimal Price { get; set; }
        public int Row { get; set; }
        public int SeatNumber { get; set; }
        public int ScreeningId { get; set; }
        public Screening? Screening { get; set; }
        public int TicketPurchaseId { get; set; }
        public TicketPurchase? TicketPurchase { get; set; }
    }
}