using System.Collections.Generic;

namespace BookClub.WebApi.Models;

public class Genre
{
    public int GenreId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public List<Book> Books { get; set; } = new();
}
