using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.Infrastructure.Data;
using LibraryManagement.Core.Entities;
using LibraryManagement.API.DTOs;

namespace LibraryManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanController : ControllerBase
    {
        private readonly LibraryDbContext _context;

        public LoanController(LibraryDbContext context)
        {
            _context = context;
        }
        
        
        // GET: api/loan
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoanDto>>> GetLoans()
        {
            var loans = await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.User)
                .ToListAsync();

            var loanDtos = loans.Select(l => new LoanDto
            {
                Id = l.Id,
                BookId = l.BookId,
                BookTitle = l.Book.Title,
                UserId = l.UserId,
                Username = l.User.Username,
                LoanDate = l.LoanDate,
                DueDate = l.DueDate,
                ReturnDate = l.ReturnDate,
                IsReturned = l.IsReturned,
                IsOverdue = l.IsOverdue
            });

            return Ok(loanDtos);
        }

        // POST: api/loan
        [HttpPost]
        public async Task<ActionResult<LoanDto>> CreateLoan(CreateLoanDto createLoanDto)
        {
            // Sprawdź czy książka istnieje
            var book = await _context.Books.FindAsync(createLoanDto.BookId);

            if (book == null)
                return NotFound($"Book with id {createLoanDto.BookId} not found");

            // Sprawdź czy użytkownik istnieje
            var user = await _context.Users.FindAsync(createLoanDto.UserId);

            if (user == null)
                return NotFound($"User with id {createLoanDto.UserId} not found");

            // Sprawdź dostępność książki
            if (book.QuantityAvailable <= 0)
                return BadRequest("Book is not available");

            var loan = new Loan
            {
                BookId = book.Id,
                UserId = user.Id,
                LoanDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(14)
            };

            // Zmniejsz ilość dostępnych egzemplarzy
            book.QuantityAvailable--;

            _context.Loans.Add(loan);

            await _context.SaveChangesAsync();

            return Ok(new LoanDto
            {
                Id = loan.Id,
                BookId = book.Id,
                BookTitle = book.Title,
                UserId = user.Id,
                Username = user.Username,
                LoanDate = loan.LoanDate,
                DueDate = loan.DueDate,
                ReturnDate = loan.ReturnDate,
                IsReturned = loan.IsReturned,
                IsOverdue = loan.IsOverdue
            });
        }
        
        // POST: api/loan/1/return
        [HttpPost("{id}/return")]
        public async Task<IActionResult> ReturnBook(int id)
        {
            var loan = await _context.Loans
                .Include(l => l.Book)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (loan == null)
                return NotFound($"Loan with id {id} not found");

            if (loan.ReturnDate != null)
                return BadRequest("Book already returned");

            loan.ReturnDate = DateTime.UtcNow;

            // zwiększ dostępne egzemplarze
            loan.Book.QuantityAvailable++;

            await _context.SaveChangesAsync();

            return Ok("Book returned successfully");
        }
        
        
        // GET: api/loan/1
        [HttpGet("{id}")]
        public async Task<ActionResult<LoanDto>> GetLoan(int id)
        {
            var loan = await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (loan == null)
                return NotFound($"Loan with id {id} not found");

            return Ok(new LoanDto
            {
                Id = loan.Id,
                BookId = loan.BookId,
                BookTitle = loan.Book.Title,
                UserId = loan.UserId,
                Username = loan.User.Username,
                LoanDate = loan.LoanDate,
                DueDate = loan.DueDate,
                ReturnDate = loan.ReturnDate,
                IsReturned = loan.IsReturned,
                IsOverdue = loan.IsOverdue
            });
        }
        
        // GET: api/loan/active
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<LoanDto>>> GetActiveLoans()
        {
            var loans = await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.User)
                .Where(l => l.ReturnDate == null)
                .ToListAsync();

            var loanDtos = loans.Select(l => new LoanDto
            {
                Id = l.Id,
                BookId = l.BookId,
                BookTitle = l.Book.Title,
                UserId = l.UserId,
                Username = l.User.Username,
                LoanDate = l.LoanDate,
                DueDate = l.DueDate,
                ReturnDate = l.ReturnDate,
                IsReturned = l.IsReturned,
                IsOverdue = l.IsOverdue
            });

            return Ok(loanDtos);
        }
        
        
        // GET: api/loan/overdue
        [HttpGet("overdue")]
        public async Task<ActionResult<IEnumerable<LoanDto>>> GetOverdueLoans()
        {
            var loans = await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.User)
                .Where(l => l.ReturnDate == null &&
                            l.DueDate < DateTime.UtcNow)
                .ToListAsync();

            var loanDtos = loans.Select(l => new LoanDto
            {
                Id = l.Id,
                BookId = l.BookId,
                BookTitle = l.Book.Title,
                UserId = l.UserId,
                Username = l.User.Username,
                LoanDate = l.LoanDate,
                DueDate = l.DueDate,
                ReturnDate = l.ReturnDate,
                IsReturned = l.IsReturned,
                IsOverdue = l.IsOverdue
            });

            return Ok(loanDtos);
        }
    }
    
    
}