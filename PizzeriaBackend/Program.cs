using Pizzeria.Data;
using PizzeriaBackend.Data;
using PizzeriaBackend.Services;
using static PizzeriaBackend.Services.JwtService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();

builder.Services.AddSingleton<Database>();
builder.Services.AddControllers();

builder.Services.AddScoped<JwtService>();

builder.Services.AddScoped<IJwtService, JwtService>();

builder.Services.AddScoped<ReviewRepository>();

builder.Services.AddScoped<IReviewRepository, ReviewRepository>();

builder.Services.AddScoped<IFoodRepository, PizzeriaBackend.Data.FoodRepository>();

builder.Services.AddScoped<ICartRepository, CartRepository>();

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
