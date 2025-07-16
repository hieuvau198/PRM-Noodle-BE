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
using PRM.Noodle.BE.Service.Users.Mappings;
using PRM.Noodle.BE.Service.Users.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using PRM.Noodle.BE.Service.Payments.Services;
using PRM.Noodle.BE.Service.Payments.Mappings;
using PRM.Noodle.BE.Service.Reports.Mappings;
using PRM.Noodle.BE.Service.Reports.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
#region Add Sign-in UI for Swagger page
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });

    // Add JWT token support to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please insert Access Token with 'Bearer ' prefix, Ex: 'Bearer ABCXYZ'",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

#endregion

#region Configure DbContext

builder.Services.AddDbContext<SpicyNoodleDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));
#endregion

#region Configure JWT Authentication

// Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"] ??
                throw new InvalidOperationException("JWT Secret is not configured"))
        ),
        ClockSkew = TimeSpan.Zero // Optional: removes default 5-minute tolerance
    };
});

// Add Authorization
builder.Services.AddAuthorization();
#endregion

#region Register Repositories

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
#endregion

#region Register Mappings
builder.Services
    .AddAutoMapper(
    typeof(ProductMappingProfile), 
    typeof(ToppingMappingProfile), 
    typeof(ComboMappingProfile),
    typeof(OrderMappingProfile),
    typeof(UserMappingProfile),
    typeof(PaymentMappingProfile),
    typeof(ReportMappingProfile)
    );

#endregion  

#region Register Services

builder.Services.AddHttpClient<IChatService, ChatService>();
builder.Services.AddScoped<IChatService, ChatService>();

builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddScoped<IToppingService, ToppingService>();

builder.Services.AddScoped<IComboService, ComboService>();

builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IPaymentService, PaymentService>();

builder.Services.AddScoped<IReportService, ReportService>();

#endregion

#region Register Controllers
//builder.Services
//    .AddControllers()
//    .AddApplicationPart(typeof(ProductsController).Assembly);
#endregion

#region Setup CORS


//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowSpecificOrigin", policy =>
//    {
//        policy.WithOrigins("http://localhost:3000", "https://localhost:3001", "http://localhost:5173", "http://localhost:5174") // Add your frontend URLs
//              .AllowAnyHeader()
//              .AllowAnyMethod()
//              .AllowCredentials();
//    });
//});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
#endregion

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

//app.UseCors("AllowSpecificOrigin");
app.UseCors("AllowAll");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
