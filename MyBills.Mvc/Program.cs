using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using MyBills.Core;
using MyBills.Data.Repositories;
using MyBills.Domain.Interfaces;
using MyBills.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ILoginRegisterService, LoginRegisterService>();
builder.Services.AddScoped<IUserBillService, UserBillService>();

builder.Services.AddTransient<IBillRepository, BillRepository>();
builder.Services.AddTransient<ILogRepository, LogRepository>();
builder.Services.AddTransient<IRecurrenceTypeRepository, RecurrenceTypeRepository>();
builder.Services.AddTransient<IUserBillRecurrenceScheduleRepository, UserBillRecurrenceScheduleRepository>();
builder.Services.AddTransient<IUserBillRepository, UserBillRepository>();
builder.Services.AddTransient<IUserRepository, UserRepository>();

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath = new PathString("/home/login");
        o.LogoutPath = new PathString("/home/logout");
    });

// Configure AppSettings with connection string
AppSettings.ConnectionString = builder.Configuration.GetConnectionString("MyBillsContext");

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
