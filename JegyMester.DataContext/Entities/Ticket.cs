using JegyMester.DataContext.Enums;
using System;

namespace JegyMester.DataContext.Entities
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public TicketType Type { get; set; }
        public bool valid { get; set; }
        public int Price { get; set; }
    }
}