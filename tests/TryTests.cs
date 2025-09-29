using Xunit;
using Svan.Monads;
using OneOf.Types;
using System;

namespace Svan.Monads.UnitTests
{
    public class TryTests
    {
        [Fact]
        public void Try_can_wrap_caught_exceptions()
        {
            Try.Catching<string>(() =>
                {
                    throw new Exception("Error that should be caught");
                })
                .DoIfError((actual) => Assert.IsType<Exception>(actual))
                .Do((_) => Assert.False(true));
        }

        [Fact]
        public void Try_can_be_casted_to_result()
        {
            Try.Catching(() => "a string")
                .DoIfError((_) => Assert.True(false))
                .Do((actual) => Assert.Equal("a string", actual))
                .MapCatching<string>((value) => throw new ArgumentException("This failed"))
                .DoIfError((exception) => Assert.IsType<ArgumentException>(exception))
                .MapError((exception) =>
                {
                    Assert.Equal("This failed", exception.Message);
                    return new ErrorThatIsNotAnExceptionType();
                })
                .Do((value) => Assert.True(false));
        }

        class ErrorThatIsNotAnExceptionType { }
    }
}