using System;
using System.Collections.Generic;

namespace BookClub.WebApi.Models;

public class Meeting
{
    public int MeetingId { get; set; }
    public DateTime Date { get; set; }
    public string? Location { get; set; }
    public string? Notes { get; set; }

    public List<BookMeeting> BookMeetings { get; set; } = new();
    public List<MemberMeeting> MemberMeetings { get; set; } = new();
}
