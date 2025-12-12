// Program.cs
using Microsoft.EntityFrameworkCore;
using LibraryManagement.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// ========== SERVICES CONFIGURATION ==========
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 🔗 Database Connection - POSTGRESQL
// Pobiera connection string z appsettings.json
builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// ========== DATABASE MIGRATION ==========
// Automatycznie tworzy bazę przy starcie (TYLKO DEVELOPMENT!)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
    
    // Metoda 1: Tylko tworzy tabele (bez historii migracji)
    dbContext.Database.EnsureCreated();
    
    // Metoda 2: Jeśli chcesz pełne migracje - ODKOMENTUJ, a zakomentuj powyższą linię
    // dbContext.Database.Migrate();
    
    Console.WriteLine("✅ Database created successfully!");
}

// ========== MIDDLEWARE PIPELINE ==========
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();