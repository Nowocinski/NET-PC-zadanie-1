using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Configure postgresql
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register services
builder.Services.AddScoped<JwtTokenService>();

// Register repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Auth endpoints
app.MapPost("/api/auth/login", async (LoginRequest request, IUserRepository userRepository) =>
{
    var tokens = await userRepository.GenerateTokensAsync(request.Email, request.Password);
    
    if (!tokens.HasValue)
        return Results.Unauthorized();
    
    return Results.Ok(new LoginResponse(tokens.Value.accessToken, tokens.Value.refreshToken));
})
.WithName("Login")
.WithOpenApi();

app.Run();

record LoginRequest(string Email, string Password);
record LoginResponse(string AccessToken, string RefreshToken);