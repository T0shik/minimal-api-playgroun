using Api.Framework;

namespace Api.Services
{
    public class CreateBlogRequest : IRequest<Blog>
    {
        public string Title { get; set; }

        public async Task<IRequest> BindFrom(HttpContext ctx)
        {
            return await ctx.Request.ReadFromJsonAsync<CreateBlogRequest>();
        }
    }

    public class CreateBlog : Handler<CreateBlogRequest, Blog>
    {
        private readonly AppDbContext ctx;

        public CreateBlog(AppDbContext ctx)
        {
            this.ctx = ctx;
        }

        public override async Task<Blog> Run(CreateBlogRequest v)
        {
            var blog = new Blog
            {
                Title = v.Title
            };

            ctx.Add(blog);

            await ctx.SaveChangesAsync();

            return blog;
        }
    }
}
