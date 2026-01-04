using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.Infrastructure.Data;
using LibraryManagement.Core.Entities;
using LibraryManagement.API.DTOs;

namespace LibraryManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly LibraryDbContext _context;
        private readonly ILogger<BooksController> _logger;

        public BooksController(LibraryDbContext context, ILogger<BooksController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks()
        {
            try
            {
                var books = await _context.Books.ToListAsync();
                
                var bookDtos = books.Select(b => new BookDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    ISBN = b.ISBN,
                    PublicationYear = b.PublicationYear,
                    QuantityAvailable = b.QuantityAvailable,
                    Publisher = b.Publisher,
                    Category = b.Category,
                    IsAvailable = b.IsAvailable
                }).ToList();
                
                return Ok(bookDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting books");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookDto>> GetBook(int id)
        {
            try
            {
                var book = await _context.Books.FindAsync(id);
                
                if (book == null)
                {
                    return NotFound($"Book with id {id} not found");
                }
                
                var bookDto = new BookDto
                {
                    Id = book.Id,
                    Title = book.Title,
                    Author = book.Author,
                    ISBN = book.ISBN,
                    PublicationYear = book.PublicationYear,
                    QuantityAvailable = book.QuantityAvailable,
                    Publisher = book.Publisher,
                    Category = book.Category,
                    IsAvailable = book.IsAvailable
                };
                
                return Ok(bookDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting book with id {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/books
        [HttpPost]
        public async Task<ActionResult<BookDto>> CreateBook(CreateBookDto createBookDto)
        {
            try
            {
                // Walidacja
                if (string.IsNullOrWhiteSpace(createBookDto.Title))
                    return BadRequest("Title is required");
                
                if (string.IsNullOrWhiteSpace(createBookDto.Author))
                    return BadRequest("Author is required");
                
                if (string.IsNullOrWhiteSpace(createBookDto.ISBN))
                    return BadRequest("ISBN is required");
                
                // Sprawdź czy ISBN już istnieje
                var existingBook = await _context.Books
                    .FirstOrDefaultAsync(b => b.ISBN == createBookDto.ISBN);
                    
                if (existingBook != null)
                    return BadRequest($"Book with ISBN {createBookDto.ISBN} already exists");
                
                var book = new Book
                {
                    Title = createBookDto.Title,
                    Author = createBookDto.Author,
                    ISBN = createBookDto.ISBN,
                    PublicationYear = createBookDto.PublicationYear,
                    QuantityAvailable = createBookDto.QuantityAvailable,
                    Publisher = createBookDto.Publisher,
                    Category = createBookDto.Category
                };
                
                _context.Books.Add(book);
                await _context.SaveChangesAsync();
                
                var bookDto = new BookDto
                {
                    Id = book.Id,
                    Title = book.Title,
                    Author = book.Author,
                    ISBN = book.ISBN,
                    PublicationYear = book.PublicationYear,
                    QuantityAvailable = book.QuantityAvailable,
                    Publisher = book.Publisher,
                    Category = book.Category,
                    IsAvailable = book.IsAvailable
                };
                
                return CreatedAtAction(nameof(GetBook), new { id = book.Id }, bookDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating book");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/books/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, UpdateBookDto updateBookDto)
        {
            try
            {
                // Walidacja
                if (string.IsNullOrWhiteSpace(updateBookDto.Title))
                    return BadRequest("Title is required");
                
                if (string.IsNullOrWhiteSpace(updateBookDto.Author))
                    return BadRequest("Author is required");
                
                if (string.IsNullOrWhiteSpace(updateBookDto.ISBN))
                    return BadRequest("ISBN is required");
                
                var book = await _context.Books.FindAsync(id);
                if (book == null)
                    return NotFound($"Book with id {id} not found");
                
                // Sprawdź czy nowy ISBN nie należy do innej książki
                if (book.ISBN != updateBookDto.ISBN)
                {
                    var existingBook = await _context.Books
                        .FirstOrDefaultAsync(b => b.ISBN == updateBookDto.ISBN && b.Id != id);
                        
                    if (existingBook != null)
                        return BadRequest($"Another book with ISBN {updateBookDto.ISBN} already exists");
                }
                
                // Aktualizuj
                book.Title = updateBookDto.Title;
                book.Author = updateBookDto.Author;
                book.ISBN = updateBookDto.ISBN;
                book.PublicationYear = updateBookDto.PublicationYear;
                book.QuantityAvailable = updateBookDto.QuantityAvailable;
                book.Publisher = updateBookDto.Publisher;
                book.Category = updateBookDto.Category;
                
                await _context.SaveChangesAsync();
                
                return NoContent(); // 204 No Content
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating book with id {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            try
            {
                var book = await _context.Books.FindAsync(id);
                if (book == null)
                    return NotFound($"Book with id {id} not found");
                
                // Sprawdź czy książka jest wypożyczona
                var activeLoans = await _context.Loans
                    .AnyAsync(l => l.BookId == id && !l.IsReturned);
                    
                if (activeLoans)
                    return BadRequest("Cannot delete book that has active loans");
                
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
                
                return NoContent(); // 204 No Content
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting book with id {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/books/search?title=abc
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<BookDto>>> SearchBooks(
            [FromQuery] string? title,
            [FromQuery] string? author,
            [FromQuery] string? category)
        {
            try
            {
                var query = _context.Books.AsQueryable();
                
                if (!string.IsNullOrWhiteSpace(title))
                    query = query.Where(b => b.Title.Contains(title));
                
                if (!string.IsNullOrWhiteSpace(author))
                    query = query.Where(b => b.Author.Contains(author));
                
                if (!string.IsNullOrWhiteSpace(category))
                    query = query.Where(b => b.Category != null && b.Category.Contains(category));
                
                var books = await query.ToListAsync();
                
                var bookDtos = books.Select(b => new BookDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    ISBN = b.ISBN,
                    PublicationYear = b.PublicationYear,
                    QuantityAvailable = b.QuantityAvailable,
                    Publisher = b.Publisher,
                    Category = b.Category,
                    IsAvailable = b.IsAvailable
                }).ToList();
                
                return Ok(bookDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching books");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}