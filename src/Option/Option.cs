using System;

namespace OneOf.Monads
{
    /// <summary>
    /// Represents some value of type T
    /// </summary>
    public struct Some<T>
    {
        public T Value { get; }
        public Some(T value)
        {
            Value = value;
        }
    }

    /// <summary>
    /// Represents no value
    /// </summary>
    public struct None { };

    /// <summary>
    /// Union of <c>None</c> and <c>Some<T></c> with monad features for the Maybe flow control
    /// </summary>
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

        /// <summary>
        /// Returns the current value. Will throw <c>NullReferenceException</c> if current option state is None.
        /// </summary>
        new public T Value() => IsSome() ? this.AsT1.Value : throw new NullReferenceException();

        /// <summary>
        /// Bind the <c>Option<T></c> to an <c>Option<TOut></c> using a binder function. The binder function will not be executed if the current state of the option is <c>None</c>.
        /// </summary>
        /// <param name="binder">A function that returns an <c>Option<TOut></c></param>
        /// <returns>An option of the output type of the binder. </returns>
        public Option<TOut> Bind<TOut>(Func<T, Option<TOut>> binder)
            => Match(
                none => none,
                some => binder(some.Value));

        /// <summary>
        /// Map the value of the option to an <c>Option<TOut></c> using a mapping function. The mapping function will not be executed if the current state of the option is <c>None</c>.
        /// </summary>
        /// <param name="mapping">A function that returns a value of <c>TOut</c></param>
        /// <typeparam name="TOut"></typeparam>
        /// <returns>An option of the output type of the mapping</returns>
        public Option<TOut> Map<TOut>(Func<T, TOut> mapping)
            => Match(
                none => Option<TOut>.None(),
                some => Option<TOut>.Some(mapping(some.Value)));

        /// <summary>
        /// Filter the value using a filter function. The filter function will not be executed if the current state of the option is <c>None</c>.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns><c>Some</c> when filter returns true. <c>None</c> when filter returns false or current state of option is <c>None</c></returns>
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
