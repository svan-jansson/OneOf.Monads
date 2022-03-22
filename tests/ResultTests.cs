using Xunit;
using OneOf.Monads;
using OneOf.Types;
using System;

namespace OneOf.Monads.UnitTests
{
    public class ResultTests
    {
        const string ErrorMessage = "division by zero";
        private Result<string, int> Divide(int number, int by)
        {
            if (by == 0)
            {
                return new Error<string>(ErrorMessage);
            }
            else
            {
                return new Success<int>(number / by);
            }
        }

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

            Result<Exception, int> add5(int val) => new Success<int>(val + 5);
            Result<Exception, int> checkIsBelow25(int val)
            {
                if (val > 25)
                {
                    return new Error<Exception>(maxLimitException);
                }
                else
                {
                    return new Success<int>(val);
                }
            }


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

        [Fact]
        public void Use_Do_to_execute_conditional_actions()
        {
            Result<Exception, int> result = 5;

            result
                .DoIfError(_ => Assert.True(false, "this should not be executed"))
                .Do(i => Assert.Equal(5, i));
        }

        [Fact]
        public void Use_DoIfError_to_execute_conditional_actions()
        {
            Result<Exception, int> result = new Exception("this is an error");

            result
                .DoIfError(error => Assert.Equal("this is an error", error.Message))
                .Do(i => Assert.True(false, "this should not be executed"));
        }

        [Fact]
        public void Result_can_be_downcasted_to_option()
        {
            Result<Exception, int> result = new Exception("this is an error");
            result.ToOption().Switch(
                none => Assert.True(true, "Error should map to None"),
                some => Assert.True(false, "this should not be executed"));

            result = 5;
            result.ToOption().Switch(
                none => Assert.True(false, "this should not be executed"),
                some => Assert.Equal(5, some.Value));
        }
    }
}