using Api;
using Api.Framework;
using Api.Services;
using static Api.EndpointAuthenticationDeclaration;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApiServices();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

Anonymous(
    app.MapGet<GetBlogs.Request>("/blogs"),
    app.MapGet<GetBlogRequest>("/blogs/{slug}")
    );

Admin(
    app.MapGet<GetBlogs.Request>("/admin/blogs"),
    app.MapGet<GetBlogRequest>("/admin/blogs/{id}"),
    // curl -i -X POST -H "Content-Type: application/json" -d "{\"title\":\"boi\"}" http://localhost:5000/admin/blogs
    app.MapPost<CreateBlogRequest>("/admin/blogs")
);

app.Run();