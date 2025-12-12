namespace LibraryManagement.API.DTOs
{
    public class BookDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public int PublicationYear { get; set; }
        public int QuantityAvailable { get; set; }
        public string? Publisher { get; set; }
        public string? Category { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class CreateBookDto
    {
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public int PublicationYear { get; set; }
        public int QuantityAvailable { get; set; }
        public string? Publisher { get; set; }
        public string? Category { get; set; }
    }

    public class UpdateBookDto
    {
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public int PublicationYear { get; set; }
        public int QuantityAvailable { get; set; }
        public string? Publisher { get; set; }
        public string? Category { get; set; }
    }
}
