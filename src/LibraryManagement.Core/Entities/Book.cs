using System.Collections.Generic;

namespace LibraryManagement.Core.Entities
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public int PublicationYear { get; set; }
        public int QuantityAvailable { get; set; }
        public string? Publisher { get; set; }
        public string? Category { get; set; }
        
        // Obliczana właściwość
        public bool IsAvailable => QuantityAvailable > 0;
        
        // Relacja
        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
        
        // Konstruktor
        public Book()
        {
            Title = string.Empty;
            Author = string.Empty;
            ISBN = string.Empty;
        }
    }
}