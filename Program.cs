// -------------------- USING STATEMENTS --------------------

// Gives access to AppDbContext (database context)
using ManuBackend.Data;

// Gives access to repository interfaces & implementations
using ManuBackend.Repository;

// Gives access to service interfaces & implementations
using ManuBackend.Services;

// Required for JWT authentication
using Microsoft.AspNetCore.Authentication.JwtBearer;

// Required for Entity Framework Core (SQL Server)
using Microsoft.EntityFrameworkCore;

// Required for JWT token validation classes
using Microsoft.IdentityModel.Tokens;

// Used to convert string → byte[] (needed for JWT secret key)
using System.Text;



// ------------------------------------------------------------
// Create the WebApplication builder object
// This is the starting point of the ASP.NET Core application.
// It sets up services, configuration, logging, etc.
var builder = WebApplication.CreateBuilder(args);



// ============================================================
// -------------------- DATABASE CONFIGURATION ----------------
// ============================================================

// AddDbContext registers AppDbContext into Dependency Injection container.
// That means whenever a controller asks for AppDbContext,
// ASP.NET Core automatically creates and provides it.

builder.Services.AddDbContext<AppDbContext>(options =>

    // UseSqlServer tells EF Core:
    // "We are using SQL Server as database provider"
    options.UseSqlServer(

        // GetConnectionString reads from appsettings.json
        // Example:
        // "ConnectionStrings": {
        //     "DefaultConnection": "Server=.;Database=ManuDB;Trusted_Connection=True;"
        // }
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);



// ============================================================
// -------------------- DEPENDENCY INJECTION ------------------
// ============================================================

// AddScoped means:
// - Create ONE object per HTTP request.
// - Same object reused inside that request.
// - New object for next request.

// Register Authentication Repository
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

// Register Authentication Service
builder.Services.AddScoped<IAuthService, AuthService>();

// Register Product Repository
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// Register Product Service
builder.Services.AddScoped<IProductService, ProductService>();

/*
Why Dependency Injection?

Instead of writing:
    var repo = new AuthRepository();

We let ASP.NET Core automatically create and inject it.

This improves:
- Loose coupling
- Testability
- Clean architecture
*/



// ============================================================
// -------------------- JWT AUTHENTICATION --------------------
// ============================================================

// Read JWT values from appsettings.json
// Example:
/*
"Jwt": {
  "Key": "THIS_IS_MY_SECRET_KEY_123456",
  "Issuer": "ManuBackend",
  "Audience": "ManuBackendUsers"
}
*/

// "!" means: we are sure value is NOT null
var jwtKey = builder.Configuration["Jwt:Key"]!;
var jwtIssuer = builder.Configuration["Jwt:Issuer"]!;
var jwtAudience = builder.Configuration["Jwt:Audience"]!;


// AddAuthentication enables authentication in the project
builder.Services.AddAuthentication(options =>
{
    // Default scheme = JWT
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Configure how token should be validated
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // Validate who created the token
        ValidateIssuer = true,

        // Validate who the token is meant for
        ValidateAudience = true,

        // Check if token expired
        ValidateLifetime = true,

        // Validate the signature using secret key
        ValidateIssuerSigningKey = true,

        // Expected Issuer
        ValidIssuer = jwtIssuer,

        // Expected Audience
        ValidAudience = jwtAudience,

        // Convert secret key string → byte[]
        // JWT signature is validated using this key
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey)
        )
    };
});



/*
JWT Flow (Important for Interview):

1. User logs in
2. Server generates JWT token
3. Client stores token (usually in localStorage)
4. Client sends token in Header:

   Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...

5. Server validates token before allowing access
*/



// ============================================================
// -------------------- CORS CONFIGURATION --------------------
// ============================================================

// CORS = Cross-Origin Resource Sharing
// Needed when frontend (React) runs on different port

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy
            .AllowAnyOrigin()   // Allow requests from any frontend
            .AllowAnyHeader()   // Allow all headers
            .AllowAnyMethod();  // Allow GET, POST, PUT, DELETE
    });
});

/*
In production:
Instead of AllowAnyOrigin()
Use:
.WithOrigins("https://yourfrontend.com")
*/



// ============================================================
// -------------------- CONTROLLERS ----------------------------
// ============================================================

// Enables controller support
builder.Services.AddControllers();



// ============================================================
// -------------------- BUILD APPLICATION ----------------------
// ============================================================

// Build the app
var app = builder.Build();



// ============================================================
// -------------------- MIDDLEWARE PIPELINE --------------------
// ============================================================

/*
Middleware = Components that handle HTTP request pipeline.
Request flows through middleware one by one.
*/

// Redirect HTTP → HTTPS
app.UseHttpsRedirection();


// IMPORTANT:
// CORS must come BEFORE Authentication
app.UseCors("AllowReactApp");


// Enable authentication middleware
app.UseAuthentication();

// Enable authorization middleware
app.UseAuthorization();


// Map controller routes (like /api/auth/login)
app.MapControllers();


// Start the application
app.Run();
