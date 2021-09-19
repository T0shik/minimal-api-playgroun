using Api.Framework;
using FluentValidation;
using System.Linq.Expressions;

namespace Api.Framework
{
    public static class WebApplicationExtensions
    {
        public static WebApplicationBuilder AddServices<T>(this WebApplicationBuilder builder)
        {
            var services = typeof(T).Assembly.GetTypes()
                .Select(t => (t, t.BaseType))
                .Where((tuple) => tuple.BaseType != null)
                .Where((tuple) => tuple.BaseType.IsGenericType && tuple.BaseType.GetGenericTypeDefinition().IsEquivalentTo(typeof(Handler<,>)));

            foreach (var s in services)
            {
                builder.Services.AddTransient(s.Item2, s.Item1);
            }

            return builder;
        }

        public static WebApplicationBuilder AddValidation<T>(this WebApplicationBuilder builder)
        {
            var validators = typeof(Blog).Assembly.GetTypes()
                .Select(t => (t, t.BaseType))
                .Where((tuple) => tuple.BaseType != null)
                .Where((tuple) => tuple.BaseType.IsGenericType
                    && tuple.BaseType.IsAbstract
                    && tuple.BaseType.GetGenericTypeDefinition().IsEquivalentTo(typeof(AbstractValidator<>)))
                .Select((tuple) => (tuple.t, tuple.BaseType.GetGenericArguments()[0]));

            foreach (var v in validators)
            {
                var validorInterfaceType = typeof(IValidator<>).MakeGenericType(v.Item2);
                builder.Services.AddTransient(validorInterfaceType, v.Item1);
            }

            return builder;
        }

        public static IEndpointConventionBuilder MapGet<TResponse>(
            this WebApplication app,
            string pattern,
            IRequest<TResponse> request
            )
        {
            return app.MapGet(pattern, CreateRequestDelegate(request));
        }

        public static IEndpointConventionBuilder MapPost<TResponse>(
            this WebApplication app,
            string pattern,
            IRequest<TResponse> request
            )
        {
            return app.MapPost(pattern, CreateRequestDelegate(request));
        }

        private static RequestDelegate CreateRequestDelegate<TResponse>(
            IRequest<TResponse> instance
            )
        {
            var ctor = instance.GetType().GetConstructors()[0];
            var newExp = Expression.New(ctor);
            var lambda = Expression.Lambda(newExp);
            var compiled = lambda.Compile();
            var requestFactory = (Func<IRequest<TResponse>>)compiled;

            return async ctx =>
            {
                var request = requestFactory();
                await request.BindFrom(ctx);

                var requestType = request.GetType();

                var validorInterfaceType = typeof(IValidator<>).MakeGenericType(requestType);
                var validator = (IValidator) ctx.RequestServices.GetService(validorInterfaceType);
                if(validator != null)
                {
                    var context = new ValidationContext<object>(request);
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

                var handlerType = typeof(Handler<,>).MakeGenericType(requestType, typeof(TResponse));

                var handler = (IHandler<TResponse>)ctx.RequestServices.GetService(handlerType);

                var result = await handler.RunAsync(request);
                await ctx.Response.WriteAsJsonAsync(result);
            };
        }

    }
}