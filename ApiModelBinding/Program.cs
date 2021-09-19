using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://127.0.0.1:5003");

var app = builder.Build();

app.MapGet("/blogs/{id}", (int id) => new Blog { 
    Id = id, Title = "wowowo w__" + id
});

app.Run();

public class Blog
{
    public int Id { get; set; }
    public string Title { get; set; }

    public List<Post> Posts { get; set; } = new();
}

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
}