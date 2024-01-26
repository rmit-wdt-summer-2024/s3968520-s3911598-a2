using System.Text;
using AdminWebAPI.Data;
using AdminWebAPI.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using AdminWebAPI.Services;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("BankingAppContext");
// Add services to the container.
builder.Services.AddDbContext<BankingAppContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ILoginRepository, LoginRepository>();


//builder.Services.AddTransient<MovieManager>();

builder.Services.AddControllers();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

// Read JWT settings from appsettings.json
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();

// Add JWT authentication services
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; 
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error"); // Add a generic error handler route
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Seed Data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        SeedData.Initialize(services);
    }
    catch(Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}


// Add CORS support
// builder.Services.AddCors(options =>
// {
//     options.AddPolicy("AllowAdminSite", builder =>
//     {
//         builder.WithOrigins("http://localhost:5218") // may be wrong 
//             .AllowAnyHeader()
//             .AllowAnyMethod();
//     });
// });

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();