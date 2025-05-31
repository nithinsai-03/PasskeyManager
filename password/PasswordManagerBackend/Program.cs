using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PasswordManagerBackend.Services; // Reference your Services namespace

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSingleton<PasswordService>(); // Register your PasswordService for Dependency Injection

// Configure CORS (Cross-Origin Resource Sharing)
// This is crucial for local development where frontend is served from file system or different port
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            // In a production environment, replace AllowAnyOrigin() with specific origins like "http://localhost:3000" or your deployed frontend URL
            policy.AllowAnyOrigin()
                  .AllowAnyMethod() // Allow GET, POST, DELETE, etc.
                  .AllowAnyHeader(); // Allow all headers
        });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); // Redirects HTTP requests to HTTPS

app.UseCors(); // Apply the CORS policy

app.UseAuthorization(); // If you had authentication/authorization, this would be active

app.MapControllers(); // Maps controller actions to routes

app.Run(); // Starts the web application