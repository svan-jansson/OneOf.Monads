# Monads Based on the OneOf Union Type

This library adds common monads to the fantastic [OneOf](https://github.com/mcintyre321/OneOf) union type library.

## Installation

```bash
dotnet add package OneOf.Monads
```

## The Option Monad

The `Option<T>` monad extends `OneOf<None, Some<T>>` and is modeled after [F#'s Option Type](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/options). It is functionally similar to [Haskell's Maybe Monad](https://wiki.haskell.org/Maybe). 

This monad provides a mechanism for conditional execution in a workflow/pipeline-style manner. Great for readability and error handling without `try`/`catch`.

### Option.Bind Example

`Bind` is used to create a contract that will resolve to `Some<T>` when all checks have passed. It will resolve to `None` if a `Bind` statement returns `None`. All subsequent `Bind` statements will be skipped.

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

### Option.Map Example

`Map` is used to map a regular value of type `T` to an `Option<T>`. In the example below it is combined with the `Bind` and `Match` functions to apply type and format conversions. For instance it takes the string output of `int.ToString()` and returns an `Option<string>` that can be used to continue the pipeline. `Map` will not execute if the current value is `None`, instead it will simply resolve to `None`, meaning that the pipeline will not break.

```csharp
using OneOf.Monads

[Fact]
public void Convert_to_option_type_using_map()
{
    var expected = "~20~";
    var option = Option<int>.Some(20);

    var actual = option
                    .Bind(IsGreaterThan10)
                    .Bind(IsEven)
                    .Map(i => i.ToString())
                    .Map(s => $"~{s}~")
                    .Match(
                        none => "could not convert number",
                        some => some.Value);

    Assert.Equal(expected, actual);
}
```
## The Result Monad

The `Result<TError, TSuccess>` monad is similar to the `Option<T>` monad, but it also defines a value for the negative case, expressed as `TError`. Instead of the `Bind` function, it uses the control flow semantic `AndThen`. It also has the `GetOrElse` function that is used to define a fallback value for a pipeline. This monad is inspired by Kotlin and provides readable data transformation pipelines and monadic error handling.

### Result.AndThen Example

Here is an example of a control flow that uses `AndThen` in combination with `Map` and `Switch`. `AndThen` can be chained and will not execute if the previous step returns the error case `Error<TError>`.

```csharp
const string ErrorMessage = "division by zero";
private Result<string, int> Divide(int number, int by)
    => by == 0
        ? new Error<string>(ErrorMessage)
        : new Success<int>(number / by);

[Fact]
public void Success_and_error_both_have_values()
{
    var expectedSuccess = (12 / 2 / 2) * 2;
    var expectedError = ErrorMessage;

    Action<int> doMath = (divideBy)
        => Divide(12, divideBy)
            .AndThen(result => Divide(result, 2))
            .Map(result => result * 2)
            .Switch(
                error => throw new DivideByZeroException(error.Value),
                success => Assert.Equal(expectedSuccess, success.Value));

    doMath(2);

    var exception = Record.Exception(() => doMath(0));
    Assert.IsType<DivideByZeroException>(exception);
    Assert.Equal(expectedError, exception.Message);
}
```

### Result.GetOrElse Example

This example demonstrates both how to define a fallback function and how to use the `TError` value, to provide logic on failure.

```csharp
[Fact]
public void GetOrElse_lets_you_define_fallback_values()
{
    var maxLimitException = new Exception();
    maxLimitException.Data.Add("max", 25);

    Func<int, Result<Exception, int>> add5 = (val) => new Success<int>(val + 5);
    Func<int, Result<Exception, int>> checkIsBelow25 = (val) =>
            val > 25
            ? new Error<Exception>(maxLimitException)
            : new Success<int>(val);

    Func<int, int> add10ReturnMax25 = (start)
        => Result<Exception, int>.Success(start)
            .AndThen(add5)
            .AndThen(add5)
            .AndThen(checkIsBelow25)
            .GetOrElse(exception => (int)exception.Data["max"]);

    Assert.Equal(20, add10ReturnMax25(10));
    Assert.Equal(25, add10ReturnMax25(15));
    Assert.Equal(25, add10ReturnMax25(20));
}
```