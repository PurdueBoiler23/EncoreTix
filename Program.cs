var builder = WebApplication.CreateBuilder(args);

// Load the configuration from appsettings.json
var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddHttpClient<TicketmasterService>(client =>
{
    client.BaseAddress = new Uri("https://app.ticketmaster.com/");
});

// Access the API key from configuration
string apiKey = configuration["Ticketmaster:ApiKey"];
builder.Services.AddSingleton(new TicketmasterService(new HttpClient(), apiKey));

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
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
    pattern: "{controller=Attractions}/{action=Index}/{id?}");

app.Run();
