using Microsoft.EntityFrameworkCore;
using PRM.Noodle.BE.Share.Data;
using PRM.Noodle.BE.Share.Interfaces;
using PRM.Noodle.BE.Share.Persistence;
using PRM.Noodle.BE.Share.Repositories;
using Services.Interfaces;
using Services.Mappings;
using Services.Services;
using PRM.Noodle.BE.Service.Products.Controllers;
using PRM.Noodle.BE.Service.Products.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Spicy Noodle API",
        Version = "v1",
        Description = "API for Spicy Noodle Restaurant Management System"
    });

    // Include XML comments if you have them
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

builder.Services.AddDbContext<SpicyNoodleDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddAutoMapper(typeof(ProductMappingProfile));
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IComboService, ComboService>();
builder.Services.AddScoped<IToppingService, ToppingService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddHttpClient<IChatService, ChatService>();
builder.Services.AddScoped<IChatService, ChatService>();

builder.Services.AddScoped<IProductsService, ProductsService>();
builder.Services.AddControllers()
    .AddApplicationPart(typeof(ProductsController).Assembly);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://localhost:3001", "http://localhost:5173", "http://localhost:5174") // Add your frontend URLs
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (true)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Spicy Noodle API V1");
        c.RoutePrefix = string.Empty; // Makes Swagger UI available at the root
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigin");

app.UseAuthorization();

app.MapControllers();

app.Run();
