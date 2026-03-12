using BookAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Register services
builder.Services.AddSingleton<IBookService, BookService>();

// Configure CORS for Angular
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins(
            "http://localhost:4200",     // Angular dev server
            "https://localhost:4200"      // HTTPS version if using
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();  // If you need cookies/auth
    });

    // For development only - allows all origins
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

// Use CORS - choose one based on your needs
app.UseCors("AllowAngular");  // For production with Angular
// app.UseCors("AllowAll");   // For development only

app.UseAuthorization();

app.MapControllers();

// Root endpoint
app.MapGet("/", () => Results.Ok(new
{
    Message = "Book Management API is running",
    Version = "1.0.0",
    Endpoints = new
    {
        GetAllBooks = "GET /api/books",
        GetBook = "GET /api/books/{id}",
        CreateBook = "POST /api/books",
        UpdateBook = "PUT /api/books/{id}",
        DeleteBook = "DELETE /api/books/{id}",
        SearchBooks = "GET /api/books/search?term={term}"
    }
}));

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new
{
    Status = "Healthy",
    Timestamp = DateTime.UtcNow,
    Environment = app.Environment.EnvironmentName
}));

app.Run();