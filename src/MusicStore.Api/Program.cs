using Microsoft.EntityFrameworkCore;
using MusicStore.Api.Middleware;
using MusicStore.Persistence;
using MusicStore.Repositories;
using MusicStore.Services;
using MusicStore.Services.Mappings;

var builder = WebApplication.CreateBuilder(args);


//CORS
const string corsConfiguration = "MusicStoreCors";
builder.Services.AddCors(setup =>
{
    setup.AddPolicy(corsConfiguration, policy =>
    {
        policy.AllowAnyOrigin(); // Que cualquiera pueda consumir el API
        policy.AllowAnyHeader().WithExposedHeaders("TotalRecordsQuantity");
        policy.AllowAnyMethod();
    });
});

// Add services to the container.
builder.Services.AddOpenApi();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Connection string
var connectionString = builder.Configuration.GetConnectionString("defaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

// Repositories dependency injection
builder.Services.AddScoped<IGenreRepository, GenreRepository>();
builder.Services.AddScoped<IConcertRepository, ConcertRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ISaleRepository, SaleRepository>();

// Dependency Injection Unit Of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Services Dependency Injection
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<ISaleService, SaleService>();

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
