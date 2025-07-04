using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using TechStrat.RGPT.UI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

string[] domains = { "http://localhost:5600"};


builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "UISpecificOrigins",
                      policy =>
                      {
                          policy
                          .WithOrigins(domains)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                      });
});

builder.Services.AddControllersWithViews();

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 2L * 1024 * 1024 * 1024; 
});


builder.Services.AddHttpClient();
builder.Services.AddScoped<IFreeformService, FreeformService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
            options.SlidingExpiration = true;
        });

builder.Services.AddAuthorization();

builder.Services.AddHttpContextAccessor();
builder.Services.AddSession();
builder.Services.AddSignalR();
var app = builder.Build();

app.UseDeveloperExceptionPage();

if (!app.Environment.IsDevelopment())
{
   app.UseHsts();
}


app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();
app.UseCors("UISpecificOrigins");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

