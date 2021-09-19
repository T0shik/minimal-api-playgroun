using Api.Framework;

namespace Api.Services
{
    public class CreateBlogRequest : IRequest<Blog>
    {
        public Task BindFrom(HttpContext ctx)
        {
            return Task.CompletedTask;
        }
    }

    public class CreateBlog : Handler<CreateBlogRequest, Blog>
    {
        public override Task<Blog> Run(CreateBlogRequest v)
        {
            throw new NotImplementedException();
        }
    }
}
