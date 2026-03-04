namespace JegyMester.DataContext.Entities
{
    public class Screening
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Room { get; set; } = string.Empty;
        public int MaxCapacity { get; set; }
        public int FilmId { get; set; }
        public Film? Film { get; set; }
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}