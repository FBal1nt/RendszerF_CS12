using JegyMester.DataContext.Enums;
using System;

namespace JegyMester.DataContext.Entities
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public TicketType Type { get; set; }
        public bool Valid { get; set; }
        public int Price { get; set; }
        public int SeatNumber { get; set; }
        public Screening Screening { get; set; }
    }
}