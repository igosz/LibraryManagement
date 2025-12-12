using Microsoft.EntityFrameworkCore;
using LibraryManagement.Core.Entities;

namespace LibraryManagement.Infrastructure.Data
{
    public class LibraryDbContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Loan> Loans { get; set; }

        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) 
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Book - konfiguracje
            modelBuilder.Entity<Book>()
                .HasIndex(b => b.ISBN)
                .IsUnique();
                
            modelBuilder.Entity<Book>()
                .Property(b => b.Title)
                .IsRequired()
                .HasMaxLength(200);
                
            modelBuilder.Entity<Book>()
                .Property(b => b.Author)
                .IsRequired()
                .HasMaxLength(100);
                
            // User - konfiguracje
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
                
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();
                
            modelBuilder.Entity<User>()
                .Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(50);
                
            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(100);
                
            // Loan - konfiguracje
            modelBuilder.Entity<Loan>()
                .HasOne(l => l.Book)
                .WithMany(b => b.Loans)
                .HasForeignKey(l => l.BookId)
                .OnDelete(DeleteBehavior.Restrict);
                
            modelBuilder.Entity<Loan>()
                .HasOne(l => l.User)
                .WithMany(u => u.Loans)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict);
                
            // Seed data (opcjonalnie - dla testów)
            modelBuilder.Entity<Book>().HasData(
                new Book { 
                    Id = 1, 
                    Title = "Clean Code", 
                    Author = "Robert C. Martin", 
                    ISBN = "9780132350884", 
                    PublicationYear = 2008, 
                    QuantityAvailable = 5 
                },
                new Book { 
                    Id = 2, 
                    Title = "The Pragmatic Programmer", 
                    Author = "Andrew Hunt, David Thomas", 
                    ISBN = "9780201616224", 
                    PublicationYear = 1999, 
                    QuantityAvailable = 3 
                }
            );
        }
    }
}