using System;
using System.Collections.Generic;

namespace BookClub.WebApi.Models;

public class Member
{
    public int MemberId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime JoinDate { get; set; }
    public string? Phone { get; set; }
    public string? Role { get; set; } = "Member";
    public string? Bio { get; set; }
    public bool IsActive { get; set; } = true;

    public List<Review> Reviews { get; set; } = new();
    public List<MemberMeeting> MemberMeetings { get; set; } = new();
}
