using Api.Framework;
using FluentValidation;
using FluentValidation.Results;

namespace Api.Services
{
    public class GetBlogRequest : IRequest<Blog>
    {
        public int Id { get; set; }

        public Task BindFrom(HttpContext ctx)
        {
            var id = (string)ctx.GetRouteValue("id");
            if(int.TryParse(id, out var idInt))
            {
                Id = idInt;
            }
            return Task.CompletedTask;
        }
    }

    public class GetBlogRequestValidation : AbstractValidator<GetBlogRequest>
    {
        public GetBlogRequestValidation()
        {
            RuleFor(r => r.Id).Must(v => v > 0).WithMessage("Id needs to be more than 0.");
        }
    }

    public class GetBlog : Handler<GetBlogRequest, Blog>
    {
        public override Task<Blog> Run(GetBlogRequest v)
        {
            return Task.FromResult(new Blog { Id = v.Id, Title = "wowowo w__" + v.Id });
        }
    }
}
