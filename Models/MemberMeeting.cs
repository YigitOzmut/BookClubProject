namespace BookClub.WebApi.Models;

public class MemberMeeting
{
    public int MemberId { get; set; }
    public int MeetingId { get; set; }

    public Member? Member { get; set; }
    public Meeting? Meeting { get; set; }
}
