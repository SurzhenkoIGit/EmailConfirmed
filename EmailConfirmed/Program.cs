using EmailConfirmed;
using EmailConfirmed.Data;
using EmailConfirmed.Models;
using EmailConfirmed.Models.Client;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EmailConfirmed.Hubs;
using Microsoft.AspNetCore.Authentication.Cookies;
using EmailConfirmed.Models.ChatBot;

var builder = WebApplication.CreateBuilder(args);


string conStr = "Server=(localdb)\\mssqllocaldb;Database=usersdb46;Trusted_Connection=True;MultipleActiveResultSets=true";
builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(conStr));

builder.Services.AddIdentity<User, IdentityRole>(options => { })
    .AddEntityFrameworkStores<ApplicationContext>()
    .AddDefaultTokenProviders();
builder.Services.Configure<IdentityOptions>(options =>
{
    options.User.AllowedUserNameCharacters = " àáâãäå¸æçèéêëìíîïðñòóôõö÷øùúûüýþÿÀÁÂÃÄÅ¨ÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ×ØÙÛÝÞßabcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPRSTUVWXYZ";
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AspAdmin", policy =>
    {
        policy.RequireRole("Àäìèíèñòðàòîð");
        policy.RequireClaim("Admin-Skill", "EmailConfirmed MVC");
    });
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.Cookie.Name = "LoginCookie";
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.ExpireTimeSpan = TimeSpan.FromDays(10);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddSignalR();
builder.Services.AddScoped<ClientService>();
builder.Services.AddSingleton<LlamaService>();
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
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
    pattern: "{controller=Home}/{action=Index}/{id?}"
    );
app.MapControllerRoute(
    name: "assistant",
    pattern: "assistant/{action=SendMessage}/{id?}");

app.MapHub<ChatHub>("/chatHub");

app.MapHealthChecks("/health");

app.Run();
