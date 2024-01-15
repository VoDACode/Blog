using Blog.Server.Data;
using Blog.Server.Extensions;
using Blog.Server.Models.Configs;
using Blog.Server.Services.AuthService;
using Blog.Server.Services.HashService;
using Blog.Server.Tools;
using Microsoft.EntityFrameworkCore;
using VoDA.AspNetCore.Services.Email;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<SystemConfigModel>(builder.Configuration.GetSection("System"));
builder.Services.Configure<DefaultUserConfigModel>(builder.Configuration.GetSection("Defaults:User"));
builder.Services.Configure<JWTConfigModel>(builder.Configuration.GetSection("JWT"));
builder.Services.Configure<AuthServiceConfigModel>(builder.Configuration.GetSection("Services:Auth"));
builder.Services.Configure<EmailServiceConfigModel>(builder.Configuration.GetSection("Services:Email"));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<BlogDbContext>(options =>
{
    options.UseSqlite(builder.Configuration["DB:ConnectionString"]);
});

builder.Services.AddHashService();

builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();

builder.Services.AddAuthService();

builder.Services.AddEmailService((e) =>
{
    var configData = builder.Configuration.Get<EmailServiceConfigModel>();

    if (configData is null)
    {
        throw new Exception("EmailServiceConfigModel is null");
    }
    e.LoadFromConfig(configData);
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

using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    Preloader.Preload(scope.ServiceProvider);
}

app.Run();