namespace LibraryManagement.Core.Entities
{
    public class Loan
    {
        public int Id { get; set; }
        
        // Klucze obce
        public int BookId { get; set; }
        public int UserId { get; set; }
        
        // Nawigacja
        public Book Book { get; set; }
        public User User { get; set; }
        
        // Daty
        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        
        // Obliczana właściwość
        public bool IsReturned => ReturnDate.HasValue;
        public bool IsOverdue => !IsReturned && DateTime.UtcNow > DueDate;
        
        // Konstruktor
        public Loan()
        {
            Book = new Book();
            User = new User();
        }
    }
}