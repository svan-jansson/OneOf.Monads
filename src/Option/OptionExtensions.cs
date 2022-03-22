namespace OneOf.Monads
{
    public static class OptionExtensions
    {
        /// <summary>
        /// Converts any type <c>T</c> to <c>Option<T></c>. 
        /// </summary>
        /// <returns>Returns <c>Some<T></c> for value types.
        /// Returns <c>Some<T></c> for reference types that are not <c>default</c> and <c>None</c> for reference types that are <c>default</c>.</returns>
        public static Option<T> ToOption<T>(this T value)
        {
            Option<T> result;

            if (typeof(T).IsValueType)
            {
                result = Option<T>.Some(value);
            }
            else
            {
                result = value as object == default
                    ? Option<T>.None()
                    : Option<T>.Some(value);
            }

            return result;
        }
    }
}
