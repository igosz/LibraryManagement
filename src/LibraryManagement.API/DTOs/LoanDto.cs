namespace LibraryManagement.API.DTOs
{
    public class CreateLoanDto
    {
        public int BookId { get; set; }
        public int UserId { get; set; }
    }

    public class LoanDto
    {
        public int Id { get; set; }

        public int BookId { get; set; }
        public string BookTitle { get; set; } = string.Empty;

        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;

        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }

        public DateTime? ReturnDate { get; set; }

        public bool IsReturned { get; set; }
        public bool IsOverdue { get; set; }
    }
}