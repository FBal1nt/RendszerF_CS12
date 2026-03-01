using JegyMester.DataContext.Enums;
using System;

namespace JegyMester.DataContext.Entities
{
    public class Film
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public GenreType Genre { get; set; }
        public int DurationInMinutes { get; set; }
        public string Director { get; set; } = string.Empty;
        public DateTime ReleaseDate { get; set; }
        public ICollection<Screening> Screenings { get; set; } = new List<Screening>();
    }
}