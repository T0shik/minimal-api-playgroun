using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://127.0.0.1:5002");

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();