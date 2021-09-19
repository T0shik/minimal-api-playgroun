using Microsoft.AspNetCore.Mvc;

namespace MVC.Controllers
{
    public class BlogsController : ControllerBase
    {
        [HttpGet("/blogs/{id}")]
        public Task<Blog> GetBlog([FromQuery] GetBlogRequest v)
        {
            return Task.FromResult(new Blog { Id = v.Id, Title = "wowowo w__" + v.Id });
        }

        public class GetBlogRequest
        {
            public int Id { get; set; }
        }

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
    }
}
