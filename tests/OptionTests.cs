using Xunit;
using OneOf.Monads;

namespace OneOf.Monads.UnitTest
{
    public class OptionTests
    {
        private Option<int> IsGreaterThan10(int i)
        {
            if (i > 10)
            {
                return new Some<int>(i);
            }
            else
            {
                return new None();
            }
        }

        private Option<int> IsEven(int i)
        {
            if (i % 2 == 0)
            {
                return new Some<int>(i);
            }
            else
            {
                return new None();
            }
        }

        [Theory]
        [InlineData(12)]
        [InlineData(24)]
        public void Conditional_execution_when_contract_is_fulfilled(int evenNumber)
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

        [Theory]
        [InlineData(11)]
        [InlineData(23)]
        public void Conditional_execution_when_contract_is_not_fulfilled(int oddNumber)
        {
            var expected = 0;
            var option = Option<int>.Some(oddNumber);

            var actual = option
                            .Bind(IsGreaterThan10)
                            .Bind(IsEven)
                            .Match(
                                none => 0,
                                some => some.Value);

            Assert.Equal(expected, actual);
        }

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

        [Fact]
        public void Pipeline_does_not_break_on_None()
        {
            var expected = "could not convert number";
            var option = Option<int>.Some(19);

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
    }
}