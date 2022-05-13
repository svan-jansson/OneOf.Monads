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
    }
}
