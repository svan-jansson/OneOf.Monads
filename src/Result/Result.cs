using System;
using OneOf.Types;

namespace OneOf.Monads
{
    public class Result<TError, TSuccess> : OneOfBase<Error<TError>, Success<TSuccess>>
    {
        public Result(OneOf<Error<TError>, Success<TSuccess>> _) : base(_) { }
        public static implicit operator Result<TError, TSuccess>(Error<TError> _) => new Result<TError, TSuccess>(_);
        public static implicit operator Result<TError, TSuccess>(Success<TSuccess> _) => new Result<TError, TSuccess>(_);

        public static implicit operator Result<TError, TSuccess>(TSuccess value) => new Success<TSuccess>(value);
        public static implicit operator Result<TError, TSuccess>(TError value) => new Error<TError>(value);

        public static Result<TError, TSuccess> Error(TError value) => new Error<TError>(value);
        public static Result<TError, TSuccess> Success(TSuccess value) => new Success<TSuccess>(value);

        public bool IsError() => this.IsT0;
        public bool IsSuccess() => this.IsT1;
        public TError ErrorValue() => IsError() ? this.AsT0.Value : throw new NullReferenceException();
        public TSuccess SuccessValue() => IsSuccess() ? this.AsT1.Value : throw new NullReferenceException();

        public Result<TError, TOut> Bind<TOut>(Func<TSuccess, Result<TError, TOut>> binder)
            => Match(
                error => Result<TError, TOut>.Error(error.Value),
                success => binder(success.Value));

        public Result<TNewError, TNewSuccess> Bind<TNewError, TNewSuccess>(
            Func<TError, TNewError> mapError,
            Func<TSuccess, Result<TNewError, TNewSuccess>> binder)
            => Match(
                error => Result<TNewError, TNewSuccess>.Error(mapError(error.Value)),
                success => binder(success.Value));

        public TSuccess Unwrap(Func<TError, TSuccess> fallback)
            => Match(
                error => fallback(error.Value),
                success => success.Value);

        public Result<TError, TOut> Map<TOut>(Func<TSuccess, TOut> mapSuccess)
            => Match(
                error => Result<TError, TOut>.Error(error.Value),
                success => Result<TError, TOut>.Success(mapSuccess(success.Value)));

        public Result<TNewError, TNewSuccess> Map<TNewError, TNewSuccess>(
            Func<TError, TNewError> mapError,
            Func<TSuccess, TNewSuccess> mapSuccess)
            => Match(
                error => Result<TNewError, TNewSuccess>.Error(mapError(error.Value)),
                success => Result<TNewError, TNewSuccess>.Success(mapSuccess(success.Value)));

        /// <summary>
        /// Do let's you fire and forget an action that is executed only when the value is <see cref="TSuccess"/> 
        /// </summary>
        /// <param name="do">An action that takes a single parameter of <see cref="TSuccess"/></param>
        /// <returns>The current state of the Result</returns>
        public Result<TError, TSuccess> Do(Action<TSuccess> @do)
        {
            if (IsSuccess())
            {
                @do(this.SuccessValue());
            }

            return this;
        }

        /// <summary>
        /// Do let's you fire and forget an action that is executed only when the value is <see cref="TError"/> 
        /// </summary>
        /// <param name="do">An action that takes a single parameter of <see cref="TError"/></param>
        /// <returns>The current state of the Result</returns>
        public Result<TError, TSuccess> DoIfError(Action<TError> @do)
        {
            if (IsError())
            {
                @do(this.ErrorValue());
            }

            return this;
        }

        /// <summary>
        /// Downcast to an <see cref="Option<TSuccess>"/>. When result state <see cref="TError"/> it will cast to <see cref="None"/>.
        /// </summary>
        /// <returns></returns>
        public Option<TSuccess> ToOption()
            => Match(
                error => Option<TSuccess>.None(),
                success => Option<TSuccess>.Some(success.Value));
    }
}
