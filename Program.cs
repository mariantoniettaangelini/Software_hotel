using Software_hotel.DAO;
using Microsoft.AspNetCore.Authentication.Cookies;
using Software_hotel.Services;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
builder.Services
    .AddScoped<IAuthService, AuthService>()
    .AddScoped<IPrenontazioneDao, PrenotazioneDao>()
    .AddControllersWithViews();

// Add authentication with cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Login";
    });

// Add authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("LoggedInPolicy", policy => policy.RequireAuthenticatedUser());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Ensure UseAuthentication is called before UseAuthorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
