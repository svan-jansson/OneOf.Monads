# Monads Based on the OneOf Union Types

This library adds common monads built upon the excellent [OneOf](https://github.com/mcintyre321/OneOf) union type library.

## The Option Monad

The `Option<T>` monad extends `OneOf<None, Some<T>>` and is modeled after [F#'s Option Type](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/options). It provides a mechanism for conditional execution in a pipeline-style manner. Great for readability and error handling without `try`/`catch`.

### Option.Bind Example

Bind is used to create a contract that will resolve to `Some<T>` when all checks have passed. It will resolve to `None` if a `Bind` statement returns `None`. All subsequent `Bind` statements will be skipped.

In this example we create a contract that guarantees that a given number is greater than 10 and is even. We then pipe the result into a `Match` to conditionally execute for both cases.

```csharp
using OneOf.Monads

Option<int> IsGreaterThan10(int i)
        => i > 10 ? new Some<int>(i) : new None();

Option<int> IsEven(int i)
    => i % 2 == 0 ? new Some<int>(i) : new None();

[Theory]
[InlineData(12)]
[InlineData(24)]
void Conditional_execution_when_contract_is_fulfilled(int evenNumber)
{
    var expected = evenNumber;
    var option = Option<int>.Some(evenNumber);

    var actual = option
                    .Bind(IsGreaterThan10)
                    .Bind(IsEven)
                    .Match(
                        none => 0,
                        some => some.Value); 

    Assert.Equal(expected, actual);
}
```