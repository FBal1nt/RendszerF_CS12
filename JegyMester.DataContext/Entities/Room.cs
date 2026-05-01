namespace JegyMester.DataContext.Entities
{
    public class Room
    {
        public int Id { get; set; }
        public int RoomNumber { get; set; }
        public int CinemaId { get; set; }
        public Cinema? Cinema { get; set; }
        public int RowCount { get; set; }
        public int SeatsPerRow { get; set; }
        public ICollection<Screening> Screenings { get; set; } = new List<Screening>();
    }
}
