using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LibraryManagement.Core.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        
        [JsonIgnore]
        public string PasswordHash { get; set; }
        
        public string Role { get; set; } = "User"; // Admin/User
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Relacja
        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
        
        public User()
        {
            Username = string.Empty;
            Email = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            PasswordHash = string.Empty;
        }
    }
}