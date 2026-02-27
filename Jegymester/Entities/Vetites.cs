using System;

public class Vetites
{
    public int Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int FilmId { get; set; }
    public List<Felhasznalo> Felhasznalok { get; set; } = new List<Felhasznalo>();



}
