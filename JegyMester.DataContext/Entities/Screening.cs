namespace JegyMester.DataContext.Entities
{
    public class Screening
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int RoomId { get; set; }
        public Room? Room { get; set; }
        public int FilmId { get; set; }
        public Film? Film { get; set; }
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}