using Microsoft.EntityFrameworkCore;
using SongViewManagement.Data;
using SongViewManagement.Helper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR(); //  to include SignalR


builder.Services.AddDbContext<SongDbContext>(option =>
    option.UseSqlServer(builder.Configuration.
    GetConnectionString("DefaultConnection")));

builder.Services.AddScoped(provider => new YouTubeViewCountService(builder.Configuration["YouTube:ApiKey"]));
builder.Services.AddSingleton(YouTubeServiceInitializer.Initialize);

builder.Services.AddScoped<SongHelper>();
builder.Services.AddScoped<SongViewHelper>();
builder.Services.AddScoped<SongViewPdfDocument>();

//builder.Services.AddHostedService<ScheduledTaskService>();


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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Song}/{action=Index}/{id?}");

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<SongHub>("/songhub"); // Map the MyHub
    //endpoints.MapFallbackToFile("index.html");
});
// Map SignalR hub
//app.MapHub<MyHub>("/myHub");

app.Run();
