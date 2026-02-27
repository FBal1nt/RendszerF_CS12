using System;

public class Screening
{
    public int Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int FilmId { get; set; }
    public List<User> Felhasznalok { get; set; } = new List<User>();



}
