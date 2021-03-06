using Api.Framework;
using FluentValidation;

namespace Api.Services
{
    public class GetBlogRequest : IRequest<Blog>
    {
        public int Id { get; private set; }
        public string Claim { get; private set; }

        public Task<IRequest> BindFrom(HttpContext ctx)
        {
            var id = (string)ctx.GetRouteValue("id");
            if (int.TryParse(id, out var idInt))
            {
                Id = idInt;
            }

            Claim = ctx.User.Claims.FirstOrDefault(x => x.Type == "test").Value;

            return Task.FromResult((IRequest)this);
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
            return Task.FromResult(new Blog { Id = v.Id, Title = "wowowo w__" + v.Claim });
        }
    }
}
