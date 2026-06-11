namespace LibraryManagement.Core.Entities
{
    public class Loan
    {
        public int Id { get; set; }

        public int BookId { get; set; }
        public int UserId { get; set; }

        public Book Book { get; set; } = null!;
        public User User { get; set; } = null!;

        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        public bool IsReturned => ReturnDate.HasValue;
        public bool IsOverdue => !IsReturned && DateTime.UtcNow > DueDate;
    }
}