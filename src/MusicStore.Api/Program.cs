using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MusicStore.Api.Middleware;
using MusicStore.Entities;
using MusicStore.Persistence;
using MusicStore.Persistence.Seeders;
using MusicStore.Repositories;
using MusicStore.Services;
using MusicStore.Services.Mappings;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddOpenApi();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Load configuration appSettings.Development.json
builder.Services.Configure<AppSettings>(builder.Configuration);

// SQL Server Connection string
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("defaultConnection"));
});

// Authentication Token Configurations
builder.Services.AddIdentity<MusicStoreUserIdentity, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 6;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

var jwtSettings = builder.Configuration.GetSection("Jwt").Get<Jwt>()
                  ?? throw new InvalidOperationException("JWT not configured");
var key = Encoding.UTF8.GetBytes(jwtSettings.Key);

builder.Services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }
).AddJwtBearer(x =>
{
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// Repositories dependency injection
builder.Services.AddScoped<IGenreRepository, GenreRepository>();
builder.Services.AddScoped<IConcertRepository, ConcertRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ISaleRepository, SaleRepository>();

// Services Dependency Injection
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IConcertService, ConcertService>();
builder.Services.AddScoped<ISaleService, SaleService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IFileStorage, FileStorageAzure>();
builder.Services.AddScoped<IUserService, UserService>();

// Dependency Injection Unit Of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Data Seaders 
builder.Services.AddTransient<UserDataSeeder>();

// Auto Mapper
builder.Services.AddAutoMapper(config =>
{
    config.AddProfile<GenreMapperProfile>();
    config.AddProfile<ConcertMapperProfile>();
    config.AddProfile<SaleMapperProfile>();
});

// Http client
builder.Services.AddHttpClient();

// Application
var app = builder.Build();

// Middleware Error handler
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Initial Data Seeders
await ApplyMigrationsAndSeedAsync(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

static async Task ApplyMigrationsAndSeedAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    if (dbContext.Database.GetPendingMigrations().Any())
    {
        await dbContext.Database.MigrateAsync();
    }

    var seeder = scope.ServiceProvider.GetRequiredService<UserDataSeeder>();
    await seeder.SeedAsync();
}
