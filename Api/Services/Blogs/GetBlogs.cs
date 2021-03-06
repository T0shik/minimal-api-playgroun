using Api.Framework;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
    public class GetBlogs : Handler<GetBlogs.Request, List<Blog>>
    {
        public class Request : IRequest<List<Blog>>
        {
            public Task<IRequest> BindFrom(HttpContext ctx) => Task.FromResult((IRequest) this);
        }

        private readonly AppDbContext ctx;

        public GetBlogs(AppDbContext ctx)
        {
            this.ctx = ctx;
        }

        public override Task<List<Blog>> Run(Request v)
        {
            return ctx.Blogs.ToListAsync();
        }
    }
}
