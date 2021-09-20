using Api.Framework;
using FluentValidation;
using System.Linq.Expressions;

namespace Api.Framework
{
    public static class WebApplicationExtensions
    {
        public static IEndpointConventionBuilder MapGet<TRequest>(
            this WebApplication app,
            string pattern
            )
            where TRequest : IRequest, new()
        {
            return app.MapGet(pattern, CreateRequestDelegate<TRequest>);
        }

        public static IEndpointConventionBuilder MapPost<TRequest>(
            this WebApplication app,
            string pattern
            )
            where TRequest : IRequest, new()
        {
            return app.MapPost(pattern, CreateRequestDelegate<TRequest>);
        }

        private static async Task CreateRequestDelegate<TRequest>(HttpContext ctx)
            where TRequest : IRequest, new()
        {
            var request = new TRequest();
            var parsedRequest = await request.BindFrom(ctx);

            var requestType = request.GetType();

            var validorInterfaceType = typeof(IValidator<>).MakeGenericType(requestType);
            var validator = (IValidator)ctx.RequestServices.GetService(validorInterfaceType);
            if (validator != null)
            {
                var context = new ValidationContext<object>(parsedRequest);
                var validationResult = validator.Validate(context);
                if (!validationResult.IsValid)
                {
                    var validationErrors = new Dictionary<string, string>();

                    foreach (var error in validationResult.Errors)
                    {
                        validationErrors[error.PropertyName] = error.ErrorMessage;
                    }

                    ctx.Response.StatusCode = 400;
                    await ctx.Response.WriteAsJsonAsync(new
                    {
                        message = "failed validation.",
                        errors = validationErrors
                    });
                    return;
                }
            }

            var returnType = typeof(TRequest).GetInterfaces()[0].GetGenericArguments()[0];

            var handlerType = typeof(Handler<,>).MakeGenericType(requestType, returnType);

            var handler = (IHandler)ctx.RequestServices.GetService(handlerType);

            var result = await handler.RunAsync(parsedRequest);
            await ctx.Response.WriteAsJsonAsync(result);
        }

    }
}