using Microsoft.EntityFrameworkCore;
using BookClub.WebApi.Models;

namespace BookClub.WebApi.Data
{
    public class BookClubContext : DbContext
    {
        public BookClubContext(DbContextOptions<BookClubContext> options) : base(options) { }

        public DbSet<Genre> Genres => Set<Genre>();
        public DbSet<Member> Members => Set<Member>();
        public DbSet<Author> Authors => Set<Author>();
        public DbSet<Meeting> Meetings => Set<Meeting>();
        public DbSet<Book> Books => Set<Book>();
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<BookAuthor> BookAuthors => Set<BookAuthor>();
        public DbSet<BookMeeting> BookMeetings => Set<BookMeeting>();
        public DbSet<MemberMeeting> MemberMeetings => Set<MemberMeeting>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Genre>(g =>
            {
                g.ToTable("Genre");
                g.HasKey(x => x.GenreId);
                g.Property(x => x.GenreId).HasColumnName("Genre_ID");
                g.Property(x => x.Name).HasColumnName("Name").IsRequired().HasMaxLength(100);
                g.Property(x => x.Description).HasColumnName("Description");
            });

            modelBuilder.Entity<Member>(m =>
            {
                m.ToTable("Member");
                m.HasKey(x => x.MemberId);
                m.Property(x => x.MemberId).HasColumnName("Member_ID");
                m.Property(x => x.Name).HasColumnName("Name").IsRequired().HasMaxLength(255);
                m.Property(x => x.Email).HasColumnName("Email").IsRequired().HasMaxLength(255);
                m.Property(x => x.JoinDate).HasColumnName("Join_Date");
                m.HasIndex(x => x.Email).IsUnique();
            });

            modelBuilder.Entity<Author>(a =>
            {
                a.ToTable("Author");
                a.HasKey(x => x.AuthorId);
                a.Property(x => x.AuthorId).HasColumnName("Author_ID");
                a.Property(x => x.Name).HasColumnName("Name").IsRequired().HasMaxLength(255);
                a.Property(x => x.BirthDate).HasColumnName("Birth_Date");
                a.Property(x => x.Nationality).HasColumnName("Nationality").HasMaxLength(100);
            });

            modelBuilder.Entity<Meeting>(mm =>
            {
                mm.ToTable("Meeting");
                mm.HasKey(x => x.MeetingId);
                mm.Property(x => x.MeetingId).HasColumnName("Meeting_ID");
                mm.Property(x => x.Date).HasColumnName("Date");
                mm.Property(x => x.Location).HasColumnName("Location").HasMaxLength(255);
                mm.Property(x => x.Notes).HasColumnName("Notes");
            });

            modelBuilder.Entity<Book>(b =>
            {
                b.ToTable("Book");
                b.HasKey(x => x.BookId);
                b.Property(x => x.BookId).HasColumnName("Book_ID");
                b.Property(x => x.Title).IsRequired().HasMaxLength(255).HasColumnName("Title");
                b.Property(x => x.PublicationYear).HasColumnName("Publication_Year");
                b.Property(x => x.PageCount).HasColumnName("Page_Count");
                b.Property(x => x.ISBN).HasColumnName("ISBN");
                b.Property(x => x.GenreId).HasColumnName("Genre_ID");
                b.Property(x => x.ReviewCount).HasColumnName("Review_Count");
                b.Property(x => x.Description).HasColumnName("Description");
                b.Property(x => x.CoverImageUrl).HasColumnName("CoverImageUrl");
                b.Property(x => x.IsAvailable).HasColumnName("IsAvailable");
                b.Ignore(x => x.AverageRating);
                b.HasOne(x => x.Genre).WithMany(g => g.Books).HasForeignKey(x => x.GenreId);
            });

            modelBuilder.Entity<Review>(r =>
            {
                r.ToTable("Review", tb => tb.HasTrigger("trg_UpdateReviewCount"));

                r.HasKey(x => x.ReviewId);
                r.Property(x => x.ReviewId).HasColumnName("Review_ID");
                r.Property(x => x.Rating).HasColumnName("Rating");
                r.Property(x => x.Comment).HasColumnName("Comment");
                r.Property(x => x.DatePosted).HasColumnName("Date_Posted");
                r.Property(x => x.BookId).HasColumnName("Book_ID");
                r.Property(x => x.MemberId).HasColumnName("Member_ID");
                r.HasOne(x => x.Book).WithMany(b => b.Reviews).HasForeignKey(x => x.BookId).OnDelete(DeleteBehavior.Cascade);
                r.HasOne(x => x.Member).WithMany(m => m.Reviews).HasForeignKey(x => x.MemberId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<BookAuthor>(ba =>
            {
                ba.ToTable("BookAuthor");
                ba.HasKey(x => new { x.BookId, x.AuthorId });
                ba.Property(x => x.BookId).HasColumnName("Book_ID");
                ba.Property(x => x.AuthorId).HasColumnName("Author_ID");
                ba.HasOne(x => x.Book).WithMany(b => b.BookAuthors).HasForeignKey(x => x.BookId).OnDelete(DeleteBehavior.Cascade);
                ba.HasOne(x => x.Author).WithMany(a => a.BookAuthors).HasForeignKey(x => x.AuthorId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<BookMeeting>(bm =>
            {
                bm.ToTable("BookMeeting");
                bm.HasKey(x => new { x.BookId, x.MeetingId });
                bm.Property(x => x.BookId).HasColumnName("Book_ID");
                bm.Property(x => x.MeetingId).HasColumnName("Meeting_ID");
                bm.HasOne(x => x.Book).WithMany(b => b.BookMeetings).HasForeignKey(x => x.BookId).OnDelete(DeleteBehavior.Cascade);
                bm.HasOne(x => x.Meeting).WithMany(m => m.BookMeetings).HasForeignKey(x => x.MeetingId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<MemberMeeting>(mmc =>
            {
                mmc.ToTable("MemberMeeting");
                mmc.HasKey(x => new { x.MemberId, x.MeetingId });
                mmc.Property(x => x.MemberId).HasColumnName("Member_ID");
                mmc.Property(x => x.MeetingId).HasColumnName("Meeting_ID");
                mmc.HasOne(x => x.Member).WithMany(m => m.MemberMeetings).HasForeignKey(x => x.MemberId).OnDelete(DeleteBehavior.Cascade);
                mmc.HasOne(x => x.Meeting).WithMany(m => m.MemberMeetings).HasForeignKey(x => x.MeetingId).OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}