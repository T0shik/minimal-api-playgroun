using Api.Framework;
using Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(o => o.UseInMemoryDatabase("myDb"));
builder.AddServices<Program>()
    .AddValidation<Program>();

var app = builder.Build();

app.MapGet("/blogs", new GetBlogsRequest());
app.MapGet("/blogs/{id}", new GetBlogRequest());

app.Run();

