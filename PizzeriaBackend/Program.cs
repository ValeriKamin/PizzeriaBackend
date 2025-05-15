using Pizzeria.Data;
using PizzeriaBackend.Data;
using PizzeriaBackend.Services;
using static PizzeriaBackend.Services.JwtService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


//var builder = WebApplication.CreateBuilder(args);


//// Add services to the container.

//builder.Services.AddRazorPages();
//builder.Services.AddControllers();
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAll", policy =>
//    {
//        policy
//            .WithOrigins("null", "http://localhost:5500") // якщо використовуєш Live Server
//            .AllowAnyMethod()
//            .AllowAnyHeader();
//    });
//}); ;

//builder.Services.AddAuthentication("Bearer")
//    .AddJwtBearer("Bearer", options =>
//    {
//        options.RequireHttpsMetadata = false;
//        options.SaveToken = true;
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = true,
//            ValidateAudience = true,
//            ValidateLifetime = true,
//            ValidateIssuerSigningKey = true,
//            ValidIssuer = "PizzeriaApp",
//            ValidAudience = "PizzeriaAppUsers",
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
//        };
//    });

//builder.Services.AddAuthorization();


//builder.Services.AddSingleton<Database>();
//builder.Services.AddScoped<JwtService>();
//builder.Services.AddScoped<IJwtService, JwtService>();
//builder.Services.AddScoped<ReviewRepository>();
//builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
//builder.Services.AddScoped<IFoodRepository, PizzeriaBackend.Data.FoodRepository>();
//builder.Services.AddScoped<ICartRepository, CartRepository>();
//builder.Services.AddScoped<IOrderRepository, OrderRepository>();
//builder.Services.AddScoped<IUserRepository, UserRepository>();
//builder.Services.AddScoped<JwtService>();
//builder.Services.AddScoped<IJwtService, JwtService>();


//var app = builder.Build();

//app.UseCors("AllowAll");
//app.MapControllers();
//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseStaticFiles();

//app.UseRouting();

//app.UseAuthentication();
//app.UseAuthorization();


//app.MapRazorPages();
//app.MapControllers();

//app.Run();

////builder.Services.AddSingleton<Database>();
////builder.Services.AddControllers();
////builder.Services.AddScoped<JwtService>();
////builder.Services.AddScoped<IJwtService, JwtService>();
////builder.Services.AddScoped<ReviewRepository>();
////builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
////builder.Services.AddScoped<IFoodRepository, PizzeriaBackend.Data.FoodRepository>();
////builder.Services.AddScoped<ICartRepository, CartRepository>();
////builder.Services.AddScoped<IOrderRepository, OrderRepository>();
///
var builder = WebApplication.CreateBuilder(args);

// 1. Додаємо Razor Pages + API контролери
builder.Services.AddRazorPages();
builder.Services.AddControllers();

// 2. Додаємо CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .WithOrigins("http://localhost:5500", "null") // Live Server або file://
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// 3. Додаємо JWT аутентифікацію
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "PizzeriaApp",
            ValidAudience = "PizzeriaAppUsers",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization();

// 4. Додаємо сервіси
builder.Services.AddSingleton<Database>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<ReviewRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IFoodRepository, PizzeriaBackend.Data.FoodRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<JwtService>();

var app = builder.Build();

app.UseCors("AllowAll");

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();




