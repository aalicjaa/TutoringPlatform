using Microsoft.EntityFrameworkCore;
using TutoringPlatform.Domain.Tutors;
using TutoringPlatform.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// EF Core + SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    if (!db.TutorProfiles.Any())
    {
        db.TutorProfiles.AddRange(
            new TutorProfile { DisplayName = "Anna Nowak", Bio = "Matematyka (liceum)", HourlyRate = 80, City = "Kraków" },
            new TutorProfile { DisplayName = "Jan Kowalski", Bio = "Angielski (B2/C1)", HourlyRate = 90, City = "Warszawa" }
        );
        db.SaveChanges();
    }
}
