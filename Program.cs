using Microsoft.Extensions.Options;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Sessions (server-side, cookie key default)
builder.Services.AddSession(o =>
{
    o.Cookie.Name = ".Cart.Session";
    o.IdleTimeout = TimeSpan.FromHours(6);
});

/*
// Stripe config (read from appsettings)
builder.Services.Configure<StripeOptions>(
    builder.Configuration.GetSection("Stripe"));
builder.Services.AddSingleton(sp =>
{
    var opts = sp.GetRequiredService<IOptions<StripeOptions>>().Value;
    StripeConfiguration.ApiKey = opts.SecretKey;
    return StripeConfiguration.ApiKey;
});
*/

// Read Stripe keys from appsettings.json
var stripeSecretKey = builder.Configuration["Stripe:SecretKey"];
if (string.IsNullOrEmpty(stripeSecretKey))
    throw new Exception("Stripe SecretKey is missing in appsettings.json");

// ⚡️ Set API key globally before building app
StripeConfiguration.ApiKey = stripeSecretKey;

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    //pattern: "{controller=Shop}/{action=Index}/{id?}");
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

public class StripeOptions
{
    public string SecretKey { get; set; } = "";
    public string PublishableKey { get; set; } = "";
}



/*
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
*/