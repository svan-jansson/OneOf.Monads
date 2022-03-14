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

        public static Option<T> None() => new None();
        public static Option<T> Some(T value) => new Some<T>(value);

        public bool IsNone() => this.IsT0;
        public bool IsSome() => this.IsT1;

        new public T Value() => IsSome() ? this.AsT1.Value : throw new NullReferenceException();

        public Option<T> Bind(Func<T, Option<T>> @continue)
            => this.Match(
                none => none,
                some => @continue(some.Value));

        public Option<TOut> Map<TOut>(Func<T, TOut> @continue)
            => this.Match(
                none => Option<TOut>.None(),
                some => Option<TOut>.Some(@continue(some.Value)));

        public Option<T> Filter(Func<T, bool> filter)
            => this.Match(
                none => none,
                some => filter(some.Value) ? some : None());
    }
}
