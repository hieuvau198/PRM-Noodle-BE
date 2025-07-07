using Microsoft.EntityFrameworkCore;
using PRM.Noodle.BE.Share.Data;
using PRM.Noodle.BE.Share.Interfaces;
using PRM.Noodle.BE.Share.Persistence;
using PRM.Noodle.BE.Share.Repositories;
using PRM.Noodle.BE.Service.Products.Controllers;
using PRM.Noodle.BE.Service.Products.Services;
using PRM.Noodle.BE.Service.Products.Mappings;
using PRM.Noodle.BE.Service.Chats.Services;
using PRM.Noodle.BE.Service.Toppings.Services;
using PRM.Noodle.BE.Service.Toppings.Mappings;
using PRM.Noodle.BE.Service.Combos.Services;
using PRM.Noodle.BE.Service.Combos.Mappings;
using PRM.Noodle.BE.Service.Orders.Services;
using PRM.Noodle.BE.Service.Orders.Mappings;

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
//builder.Services.AddScoped<IProductService, ProductService>();


#region Register Controllers
//builder.Services
//    .AddControllers()
//    .AddApplicationPart(typeof(ProductsController).Assembly);
#endregion

#region Register Mappings
builder.Services
    .AddAutoMapper(
    typeof(ProductMappingProfile), 
    typeof(ToppingMappingProfile), 
    typeof(ComboMappingProfile),
    typeof(OrderMappingProfile)
    );

#endregion  

#region Register Services

builder.Services.AddHttpClient<IChatService, ChatService>();
builder.Services.AddScoped<IChatService, ChatService>();

builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddScoped<IToppingService, ToppingService>();

builder.Services.AddScoped<IComboService, ComboService>();

builder.Services.AddScoped<IOrderService, OrderService>();

#endregion




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
    app.UseDeveloperExceptionPage(); // <--- add this
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
