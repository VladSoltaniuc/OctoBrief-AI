using OctoBrief.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register HttpClient for scraping
builder.Services.AddHttpClient("Scraper", client =>
{
  client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
  client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
  client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");
  client.Timeout = TimeSpan.FromSeconds(30);
});

// Register services
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IScraperService, ScraperService>();
builder.Services.AddScoped<IAiService, AiService>();
builder.Services.AddScoped<INewsSearchService, NewsSearchService>();

var app = builder.Build();

// Configure Swagger
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();

