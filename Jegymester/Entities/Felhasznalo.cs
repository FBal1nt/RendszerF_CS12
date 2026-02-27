using Jegymester.Entities;
using System;

public class Felhasznalo
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public IEnumerable<Jegyvasarlas> Jegyvasarlasok { get; set; } = new List<Jegyvasarlas>();
    public IEnumerable<Berlet> Berletek { get; set; } = new List<Berlet>();
    public IEnumerable<Szerepkor> Szerepkorok { get; set; } = new List<Szerepkor>();
}
