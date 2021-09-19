using FluentValidation;

namespace Api.Framework
{
    public interface IRequest
    {
        Task BindFrom(HttpContext ctx);
    }
    public interface IRequest<TOut> : IRequest
    {
    }

    public interface IHandler<TOut>
    {
        public Task<TOut> RunAsync(object v);
    }

    public abstract class Handler<TIn, TOut> : IHandler<TOut>
        where TIn : IRequest<TOut>
    {
        public abstract Task<TOut> Run(TIn v);

        public Task<TOut> RunAsync(object v) => Run((TIn)v);
    }
}
