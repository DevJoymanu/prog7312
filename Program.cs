using Microsoft.EntityFrameworkCore;
using prog7312_st10161149_part1.Data;
using prog7312_st10161149_part1.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add in-memory services as singleton to maintain data across requests
builder.Services.AddSingleton<IIssueService, IssueService>();
builder.Services.AddSingleton<IEventService, EventService>();

// Add logging
builder.Services.AddLogging();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();