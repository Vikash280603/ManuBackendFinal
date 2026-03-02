// ============================================================
// USING STATEMENTS
// ============================================================

// Contains AppDbContext (EF Core database context)
using ManuBackend.Data;

// Repository layer (data access)
using ManuBackend.Repository;

// Service layer (business logic)
using ManuBackend.Services;

// Enables JWT-based authentication
using Microsoft.AspNetCore.Authentication.JwtBearer;

// Entity Framework Core + SQL Server support
using Microsoft.EntityFrameworkCore;

// Used for validating JWT tokens
using Microsoft.IdentityModel.Tokens;

// Used to convert string → byte[] (JWT secret key)
using System.Text;

// Rate limiting (protects APIs from too many requests)
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;



// ============================================================
// APPLICATION STARTUP
// ============================================================

// Creates the WebApplication builder.
// This object is responsible for:
// - Reading appsettings.json
// - Registering services
// - Preparing middleware pipeline
var builder = WebApplication.CreateBuilder(args);



// ============================================================
// DATABASE CONFIGURATION (Entity Framework Core)
// ============================================================

// Registers AppDbContext in Dependency Injection (DI)
//
// Meaning:
// Whenever a class (Controller / Repository) asks for AppDbContext,
// ASP.NET Core will automatically create and provide it.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        // Reads connection string from appsettings.json
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);



// ============================================================
// DEPENDENCY INJECTION (Repositories & Services)
// ============================================================

// AddScoped =
// - One instance per HTTP request
// - Same instance reused inside that request
// - New instance created for next request

// Auth
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Product
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

// Inventory
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IInventoryService, InventoryService>();

// Work Orders
builder.Services.AddScoped<IWorkOrderRepository, WorkOrderRepository>();
builder.Services.AddScoped<IWorkOrderService, WorkOrderService>();

// Quality Check
builder.Services.AddScoped<IQualityCheckRepository, QualityCheckRepository>();
builder.Services.AddScoped<IQualityCheckService, QualityCheckService>();

/*
Why Dependency Injection?

Instead of manually doing:
    var service = new ProductService();

ASP.NET Core creates and injects objects automatically.

Benefits:
✔ Loose coupling
✔ Easy unit testing
✔ Clean architecture
*/



// ============================================================
// RATE LIMITING (API PROTECTION)
// ============================================================

// Adds in-memory caching (required for rate limiter)
builder.Services.AddMemoryCache();

// Configure rate limiting rules
builder.Services.AddRateLimiter(options =>
{
    // HTTP status returned when limit exceeded
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // Fixed window limiter
    options.AddFixedWindowLimiter("fixed", limiter =>
    {
        // Time window length
        limiter.Window = TimeSpan.FromSeconds(10);

        // Max allowed requests per window
        limiter.PermitLimit = 50000;

        // No waiting queue
        limiter.QueueLimit = 1000;

        limiter.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
});



// ============================================================
// JWT AUTHENTICATION
// ============================================================

// Read JWT values from appsettings.json
// "!" means: we promise this value is NOT null
var jwtKey = builder.Configuration["Jwt:Key"]!;
var jwtIssuer = builder.Configuration["Jwt:Issuer"]!;
var jwtAudience = builder.Configuration["Jwt:Audience"]!;

// Enables authentication in ASP.NET Core
builder.Services.AddAuthentication(options =>
{
    // Use JWT as default authentication mechanism
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Defines how incoming JWT tokens are validated
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,            // Who created the token
        ValidateAudience = true,          // Who can use the token
        ValidateLifetime = true,          // Expiry check
        ValidateIssuerSigningKey = true,  // Signature validation

        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,

        // Secret key used to verify token signature
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey)
        )
    };  
});

/*
JWT FLOW (VERY IMPORTANT):

1. User logs in
2. Server creates JWT
3. Client stores token (localStorage)
4. Client sends token in request header:
   Authorization: Bearer <TOKEN>
5. Server validates token before executing controller
*/



// ============================================================
// CORS (Frontend Communication)
// ============================================================

// Required when frontend (React) runs on different port
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});



// ============================================================
// CONTROLLERS
// ============================================================

// Enables attribute-based controllers like [ApiController]
builder.Services.AddControllers();



// ============================================================
// BUILD APPLICATION
// ============================================================

var app = builder.Build();



// ============================================================
// MIDDLEWARE PIPELINE
// ============================================================

/*
Middleware = components that handle HTTP requests.
Request flows TOP → BOTTOM.
Order matters!
*/

// Enable CORS (must come before auth)
app.UseCors("AllowReactApp");

// Validates JWT token
app.UseAuthentication();

// Checks user permissions ([Authorize])
app.UseAuthorization();

// Maps controller routes (ex: /api/auth/login)
app.MapControllers();

// Apply rate limiting rules
app.UseRateLimiter();

// Start listening for HTTP requests
app.Run();