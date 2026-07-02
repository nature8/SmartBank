using Microsoft.AspNetCore.Authentication.Cookies;
using SmartBank.Web.Models;
using SmartBank.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// ---- Razor Pages ------------------------------------------------------------------------------
// Baseline: every page requires an authenticated user. The exact ROLE required for any given
// page is declared right on that page's PageModel via [Authorize(Roles = "...")], which keeps
// the rule next to the handler it protects instead of scattered across folder conventions
// (important here because folders like /Customers mix staff-only pages such as Index/Create
// with customer-only pages such as MyProfile).
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/");
    options.Conventions.AllowAnonymousToPage("/Account/Login");
    options.Conventions.AllowAnonymousToPage("/Account/Register");
    options.Conventions.AllowAnonymousToPage("/Account/AccessDenied");
    options.Conventions.AllowAnonymousToPage("/Error");
});

// ---- Options ----------------------------------------------------------------------------------
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

// ---- HttpContext access for the bearer-token handler ------------------------------------------
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<BearerTokenHandler>();

// ---- Typed HTTP clients pointing at the API Gateway (YARP) ------------------------------------
var gatewayBaseUrl = builder.Configuration["ApiSettings:GatewayBaseUrl"] ?? "http://localhost:5202/";

builder.Services.AddHttpClient<IAuthApiClient, AuthApiClient>(client =>
{
    client.BaseAddress = new Uri(gatewayBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
}).AddHttpMessageHandler<BearerTokenHandler>();

builder.Services.AddHttpClient<ICustomerApiClient, CustomerApiClient>(client =>
{
    client.BaseAddress = new Uri(gatewayBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
}).AddHttpMessageHandler<BearerTokenHandler>();

builder.Services.AddHttpClient<IAccountApiClient, AccountApiClient>(client =>
{
    client.BaseAddress = new Uri(gatewayBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
}).AddHttpMessageHandler<BearerTokenHandler>();

builder.Services.AddHttpClient<ITransactionApiClient, TransactionApiClient>(client =>
{
    client.BaseAddress = new Uri(gatewayBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
}).AddHttpMessageHandler<BearerTokenHandler>();

builder.Services.AddHttpClient<INotificationApiClient, NotificationApiClient>(client =>
{
    client.BaseAddress = new Uri(gatewayBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
}).AddHttpMessageHandler<BearerTokenHandler>();

// ---- Cookie authentication (the UI's own session; the JWT from AuthService rides along as a claim) ----
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
        options.SlidingExpiration = true;
        options.Cookie.Name = "SmartBank.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// ---- Middleware pipeline ----------------------------------------------------------------------
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

app.Run();
