using System.Collections.Generic;

namespace BookClub.WebApi.Models;

public class Book
{
    public int BookId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int? PublicationYear { get; set; }
    public int? PageCount { get; set; }
    public string? ISBN { get; set; }
    public int GenreId { get; set; }
    public int ReviewCount { get; set; }
    public string? Description { get; set; }
    public string? CoverImageUrl { get; set; }
    public double AverageRating { get; set; }
    public bool IsAvailable { get; set; } = true;

    public Genre? Genre { get; set; }
    public List<BookAuthor> BookAuthors { get; set; } = new();
    public List<BookMeeting> BookMeetings { get; set; } = new();
    public List<Review> Reviews { get; set; } = new();
}
