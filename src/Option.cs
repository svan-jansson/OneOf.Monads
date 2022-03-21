using System;

namespace OneOf.Monads
{
    public struct Some<T>
    {
        public T Value { get; }
        public Some(T value)
        {
            Value = value;
        }
    }

    public struct None { };

    public class Option<T> : OneOfBase<None, Some<T>>
    {
        public Option(OneOf<None, Some<T>> _) : base(_) { }
        public static implicit operator Option<T>(None _) => new Option<T>(_);
        public static implicit operator Option<T>(Some<T> _) => new Option<T>(_);
        public static implicit operator Option<T>(T _) => Some(_);

        public static Option<T> None() => new None();
        public static Option<T> Some(T value) => new Some<T>(value);

        public bool IsNone() => this.IsT0;
        public bool IsSome() => this.IsT1;

        new public T Value() => IsSome() ? this.AsT1.Value : throw new NullReferenceException();

        public Option<TOut> Bind<TOut>(Func<T, Option<TOut>> @continue)
            => Match(
                none => none,
                some => @continue(some.Value));

        public Option<TOut> Map<TOut>(Func<T, TOut> @continue)
            => Match(
                none => Option<TOut>.None(),
                some => Option<TOut>.Some(@continue(some.Value)));

        public Option<T> Filter(Func<T, bool> filter)
            => Match(
                none => none,
                some => filter(some.Value) ? some : None());

        /// <summary>
        /// Do let's you fire and forget an action that is executed only when the value is Some<T>
        /// </summary>
        /// <param name="do">An action that takes a single parameter of T</param>
        /// <returns>The current state of the Option</returns>
        public Option<T> Do(Action<T> @do)
        {
            if (IsSome())
            {
                @do(this.Value());
            }

            return this;
        }

        /// <summary>
        /// Do let's you fire and forget an action that is executed only when the value is None
        /// </summary>
        /// <param name="do">An action that takes no parameters</param>
        /// <returns>The current state of the Option</returns>
        public Option<T> DoIfNone(Action @do)
        {
            if (IsNone())
            {
                @do();
            }

            return this;
        }
    }
}
