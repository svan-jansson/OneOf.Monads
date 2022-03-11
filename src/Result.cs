using System;
using OneOf.Types;

namespace OneOf.Monads
{
    public class Result<TError, TSuccess> : OneOfBase<Error<TError>, Success<TSuccess>>
    {
        public Result(OneOf<Error<TError>, Success<TSuccess>> _) : base(_) { }
        public static implicit operator Result<TError, TSuccess>(Error<TError> _) => new Result<TError, TSuccess>(_);
        public static implicit operator Result<TError, TSuccess>(Success<TSuccess> _) => new Result<TError, TSuccess>(_);

        public static Result<TError, TSuccess> Error(TError value) => new Error<TError>(value);
        public static Result<TError, TSuccess> Success(TSuccess value) => new Success<TSuccess>(value);

        public bool IsError() => this.IsT0;
        public bool IsSuccess() => this.IsT1;
        public TError ErrorValue() => IsError() ? this.AsT0.Value : throw new NullReferenceException();
        public TSuccess SuccessValue() => IsSuccess() ? this.AsT1.Value : throw new NullReferenceException();

        public Result<TError, TSuccess> AndThen(Func<TSuccess, Result<TError, TSuccess>> then)
            => this.Match(
                error => error,
                success => then(success.Value));

        public TSuccess GetOrElse(Func<TError, TSuccess> fallback)
            => this.Match(
                error => fallback(error.Value),
                success => success.Value);

        public Result<TError, TOut> Map<TOut>(Func<TSuccess, TOut> mapSuccess)
            => this.Match(
                error => Result<TError, TOut>.Error(error.Value),
                success => Result<TError, TOut>.Success(mapSuccess(success.Value)));

        public Result<TNewError, TNewSuccess> Map<TNewError, TNewSuccess>(
            Func<TError, TNewError> mapError,
            Func<TSuccess, TNewSuccess> mapSuccess)
            => this.Match(
                error => Result<TNewError, TNewSuccess>.Error(mapError(error.Value)),
                success => Result<TNewError, TNewSuccess>.Success(mapSuccess(success.Value)));
    }
}
