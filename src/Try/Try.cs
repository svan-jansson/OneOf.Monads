using System;
using OneOf.Types;

namespace OneOf.Monads
{
    public static class Try
    {
        public static Try<TSuccess> Catching<TSuccess>(Func<TSuccess> codeBlock)
        {
            try
            {
                return codeBlock();
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
    }

    public class Try<TSuccess> : Result<Exception, TSuccess>
    {
        public Try(OneOf<Error<Exception>, Success<TSuccess>> _) : base(_) { }
        public static implicit operator Try<TSuccess>(Error<Exception> _) => new Try<TSuccess>(_);
        public static implicit operator Try<TSuccess>(Success<TSuccess> _) => new Try<TSuccess>(_);
        public static implicit operator Try<TSuccess>(TSuccess value) => new Success<TSuccess>(value);
        public static implicit operator Try<TSuccess>(Exception value) => new Error<Exception>(value);

        public Result<Exception, TSuccess> ToResult() => this;

        public Try<TOut> MapCatching<TOut>(Func<TSuccess, TOut> mapper)
            => Fold(
                 error => error,
                 success => Try.Catching(() => mapper(success)));

        public Try<TOut> Bind<TOut>(Func<TSuccess, Try<TOut>> binder)
            => Match(
                error => Result<Exception, TOut>.Error(error.Value) as Try<TOut>,
                success => binder(success.Value));

        public Try<TSuccess> BindError(Func<Exception, Try<TSuccess>> binder)
            => Match(
                error => binder(error.Value),
                success => Success(success.Value) as Try<TSuccess>);

        public new Try<TOut> Map<TOut>(Func<TSuccess, TOut> mapSuccess)
            => Match(
                error => Result<Exception, TOut>.Error(error.Value) as Try<TOut>,
                success => Result<Exception, TOut>.Success(mapSuccess(success.Value)) as Try<TOut>);

        public Try<TSuccess> MapError(Func<Exception, Exception> mapError)
            => Match(
                error => Error(mapError(error.Value)) as Try<TSuccess>,
                success => Success(success.Value) as Try<TSuccess>);

        /// <summary>
        /// Do let's you fire and forget an action that is executed only when the value is <see cref="TSuccess"/> 
        /// </summary>
        /// <param name="do">An action that takes a single parameter of <see cref="TSuccess"/></param>
        /// <returns>The current state of the Result</returns>
        public new Try<TSuccess> Do(Action<TSuccess> @do)
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
        public new Try<TSuccess> DoIfError(Action<Exception> @do)
        {
            if (IsError())
            {
                @do(this.ErrorValue());
            }

            return this;
        }


        /// <summary>
        /// Combine several results into a new result of <c>TSuccessOut</c> or <c>TError</c> if any of the provided results has an error
        /// </summary>
        public Try<TSuccessOut> Zip<TSuccessOut, TSuccessOther>(
            Try<TSuccessOther> other,
            Func<TSuccess, TSuccessOther, TSuccessOut> combine)
                => base.Zip(other, combine) as Try<TSuccessOut>;

        /// <summary>
        /// Combine several results into a new result of <c>TSuccessOut</c> or <c>TError</c> if any of the provided results has an error
        /// </summary>
        public Try<TSuccessOut> Zip<TSuccessOut, TSuccessFirstOther, TSuccessSecondOther>(
            Try<TSuccessFirstOther> firstOther,
            Try<TSuccessSecondOther> secondOther,
            Func<TSuccess, TSuccessFirstOther, TSuccessSecondOther, TSuccessOut> combine)
                => base.Zip(firstOther, secondOther, combine) as Try<TSuccessOut>;

        /// <summary>
        /// Combine several results into a new result of <c>TSuccessOut</c> or <c>TError</c> if any of the provided results has an error
        /// </summary>
        public Try<TSuccessOut> Zip<TSuccessOut, TSuccessFirstOther, TSuccessSecondOther, TSuccessThirdOther>(
            Try<TSuccessFirstOther> firstOther,
            Try<TSuccessSecondOther> secondOther,
            Try<TSuccessThirdOther> thirdOther,
            Func<TSuccess, TSuccessFirstOther, TSuccessSecondOther, TSuccessThirdOther, TSuccessOut> combine)
                => base.Zip(firstOther, secondOther, thirdOther, combine) as Try<TSuccessOut>;

        /// <summary>
        /// Combine several results into a new result of <c>TSuccessOut</c> or <c>TError</c> if any of the provided results has an error
        /// </summary>
        public Try<TSuccessOut> Zip<
            TSuccessOut,
            TSuccessFirstOther,
            TSuccessSecondOther,
            TSuccessThirdOther,
            TSuccessFourthOther>(
            Try<TSuccessFirstOther> firstOther,
            Try<TSuccessSecondOther> secondOther,
            Try<TSuccessThirdOther> thirdOther,
            Try<TSuccessFourthOther> fourthOther,
            Func<
                TSuccess,
                TSuccessFirstOther,
                TSuccessSecondOther,
                TSuccessThirdOther,
                TSuccessFourthOther,
                TSuccessOut> combine)
                    => base.Zip(firstOther, secondOther, thirdOther, fourthOther, combine) as Try<TSuccessOut>;
    }
}
