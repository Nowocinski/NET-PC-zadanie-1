using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

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

app.UseCors("AllowAngularApp");

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

app.MapPost("/api/auth/register", async (RegisterRequest request, IUserRepository userRepository) =>
{
    var user = await userRepository.RegisterAsync(request.Email, request.Password, request.Name);
    
    if (user == null)
        return Results.BadRequest(new { message = "User with this email already exists" });
    
    return Results.Ok(new RegisterResponse(user.Id, user.Email, user.Name));
})
.WithName("Register")
.WithOpenApi();

app.Run();

record LoginRequest(string Email, string Password);
record LoginResponse(string AccessToken, string RefreshToken);
record RegisterRequest(string Email, string Password, string Name);
record RegisterResponse(Guid Id, string Email, string Name);