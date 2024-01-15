using Blog.Server.Data;
using Blog.Server.Models.Configs;
using Blog.Server.Services.AuthService;
using Microsoft.EntityFrameworkCore;
using VoDA.AspNetCore.Services.Email;

var builder = WebApplication.CreateBuilder(args);

// TODO: Add configuration settings for Services:Email

builder.Services.Configure<SystemConfigModel>(builder.Configuration.GetSection("System"));
builder.Services.Configure<JWTConfigModel>(builder.Configuration.GetSection("JWT"));
builder.Services.Configure<AuthServiceConfigModel>(builder.Configuration.GetSection("Services:Auth"));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<BlogDbContext>(options =>
{
    options.UseSqlite(builder.Configuration["DB:ConnectionString"]);
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();

builder.Services.AddAuthService();

builder.Services.AddEmailService((e) =>
{
    // TODO: Add using configuration settings for Services:Email

    e.Email = builder.Configuration["Services:Email:EmailAddress"];
    e.DisplayName = builder.Configuration["Services:Email:DisplayName"];
    e.Password = builder.Configuration["Services:Email:Password"];
    e.Host = builder.Configuration["Services:Email:Host"];
    e.Port = int.Parse(builder.Configuration["Services:Email:Port"]);
    e.EnableSsl = bool.Parse(builder.Configuration["Services:Email:EnableSsl"]);
    e.UseDefaultCredentials = bool.Parse(builder.Configuration["Services:Email:UseDefaultCredentials"]);
    e.EmailTemplatesFolder = builder.Configuration["Services:Email:EmailTemplatesFolder"];
});

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
