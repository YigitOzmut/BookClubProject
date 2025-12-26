using System.Collections.Generic;

namespace BookClub.WebApi.Models;

public class Author
{
    public int AuthorId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; }
    public string? Nationality { get; set; }

    public List<BookAuthor> BookAuthors { get; set; } = new();
}
