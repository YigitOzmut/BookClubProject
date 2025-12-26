namespace BookClub.WebApi.Models;

public class BookMeeting
{
    public int BookId { get; set; }
    public int MeetingId { get; set; }

    public Book? Book { get; set; }
    public Meeting? Meeting { get; set; }
}
