using Microsoft.EntityFrameworkCore;
using BookClub.WebApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddControllersWithViews();
builder.Services.AddAntiforgery();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
    "Server=localhost;Database=BookClubDB;Trusted_Connection=True;TrustServerCertificate=True;";

builder.Services.AddDbContext<BookClubContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions => 
    {
        // Disable the OUTPUT clause to avoid conflicts with database triggers
        sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
    }));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAntiforgery();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();
Console.WriteLine(
    builder.Configuration.GetConnectionString("DefaultConnection"));
app.Run();
