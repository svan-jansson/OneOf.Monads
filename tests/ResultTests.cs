using Xunit;
using OneOf.Monads;
using OneOf.Types;

namespace OneOf.Monads.UnitTests;

public class ResultTests
{
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
}