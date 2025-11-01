using System.Security.Claims;
using System.Text;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });

builder.Services.AddAuthorization();

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
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ISubcategoryRepository, SubcategoryRepository>();

var app = builder.Build();

// Seed data
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
    
    // Seed admin user
    var adminEmail = "admin@netpc.pl";
    var existingAdmin = await userRepository.GetByEmailAsync(adminEmail);
    if (existingAdmin == null)
    {
        await userRepository.RegisterAsync(adminEmail, "**TSCBf2hS**", "Administrator");
    }
    
    // Seed categories
    if (!dbContext.Categories.Any())
    {
        var categories = new[]
        {
            new Core.Entities.Category { Id = Guid.NewGuid(), Name = "Służbowy" },
            new Core.Entities.Category { Id = Guid.NewGuid(), Name = "Prywatny" },
            new Core.Entities.Category { Id = Guid.NewGuid(), Name = "Inny" }
        };
        
        await dbContext.Categories.AddRangeAsync(categories);
        await dbContext.SaveChangesAsync();
    }
    
    // Seed subcategories
    if (!dbContext.Subcategories.Any())
    {
        var official = await dbContext.Categories.SingleOrDefaultAsync(category => category.Name == "Służbowy");

        if (official == null)
        {
            return;
        }
        
        var subcategories = new[]
        {
            new Core.Entities.Subcategory { Id = Guid.NewGuid(), Name = "Szef", CategoryId = official.Id },
            new Core.Entities.Subcategory { Id = Guid.NewGuid(), Name = "klient", CategoryId = official.Id }
        };
        
        await dbContext.Subcategories.AddRangeAsync(subcategories);
        await dbContext.SaveChangesAsync();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowAngularApp");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

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

// Category endpoints
app.MapGet("/api/categories", async (ICategoryRepository categoryRepository) =>
{
    var categories = await categoryRepository.GetAllAsync();
    return Results.Ok(categories);
})
.WithName("GetCategories")
.WithOpenApi();

// Subcategory endpoints
app.MapGet("/api/subcategories", async (GetSubcategoriesRequest request, ISubcategoryRepository subcategoryRepository) =>
{
    var subcategories = await subcategoryRepository.GetByCategoryNameAsync(request.Name);
    return Results.Ok(subcategories);
})
.WithName("GetSubcategories")
.WithOpenApi();

// Contact endpoints
app.MapGet("/api/contacts", async (IContactRepository contactRepository) =>
{
    var contacts = await contactRepository.GetAllAsync();
    return Results.Ok(contacts);
})
.WithName("GetContacts")
.WithOpenApi();

app.MapDelete("/api/contacts/{id}", async (Guid id, IContactRepository contactRepository) =>
{
    await contactRepository.DeleteAsync(id);
    return Results.NoContent();
})
.WithName("DeleteContact")
.WithOpenApi();

app.MapPost("/api/contacts", async (CreateContactRequest request, ClaimsPrincipal user, IContactRepository contactRepository) =>
{
    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    
    if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        return Results.Unauthorized();
    
    var contact = new Core.Entities.Contact
    {
        Id = Guid.NewGuid(),
        UserId = userId,
        FirstName = request.FirstName,
        LastName = request.LastName,
        Email = request.Email,
        Phone = request.Phone,
        BirthDate = DateTime.SpecifyKind(request.BirthDate, DateTimeKind.Utc),
        CategoryId = request.CategoryId,
        SubcategoryId = request.SubcategoryId,
        Password = request.Password
    };
    
    var createdContact = await contactRepository.AddAsync(contact);
    return Results.Created($"/api/contacts/{createdContact.Id}", createdContact);
})
.RequireAuthorization()
.WithName("CreateContact")
.WithOpenApi();

app.MapPut("/api/contacts/{id}", async (Guid id, UpdateContactRequest request, IContactRepository contactRepository) =>
{
    var contact = new Core.Entities.Contact
    {
        Id = id,
        UserId = request.UserId,
        FirstName = request.FirstName,
        LastName = request.LastName,
        Email = request.Email,
        Phone = request.Phone,
        BirthDate = DateTime.SpecifyKind(request.BirthDate, DateTimeKind.Utc),
        CategoryId = request.CategoryId,
        SubcategoryId = request.SubcategoryId,
        Password = request.Password
    };
    
    await contactRepository.UpdateAsync(contact);
    return Results.NoContent();
})
.RequireAuthorization()
.WithName("UpdateContact")
.WithOpenApi();

app.Run();

record LoginRequest(string Email, string Password);
record LoginResponse(string AccessToken, string RefreshToken);
record RegisterRequest(string Email, string Password, string Name);
record RegisterResponse(Guid Id, string Email, string Name);
record CreateContactRequest(string FirstName, string LastName, string Email, string Phone, DateTime BirthDate, Guid? CategoryId, Guid? SubcategoryId, string Password);
record UpdateContactRequest(Guid UserId, string FirstName, string LastName, string Email, string Phone, DateTime BirthDate, Guid? CategoryId, Guid? SubcategoryId, string Password);
record GetSubcategoriesRequest(string Name);