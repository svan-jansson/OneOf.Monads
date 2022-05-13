using Xunit;
using OneOf.Monads;
using OneOf.Types;
using System;

namespace OneOf.Monads.UnitTests
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

            Try.Catching(() => "a string")
                .DoIfError((_) => Assert.True(false))
                .Do((actual) => Assert.Equal("a string", actual));
        }
    }
}