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
                return i;
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
                return i;
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
            Option<int> option = evenNumber;

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
            Option<int> option = oddNumber;

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
            Option<int> option = 20;

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
            Option<int> option = 19;

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
        public void Use_filter_to_get_conditional_result()
        {
            var expected = 5;
            Option<int> option = 5;
            var actual = option
                .Filter(i => i > 0)
                .Filter(i => i % 2 > 0)
                .Match(
                    none => 0,
                    some => some.Value);

            Assert.Equal(expected, actual);

            expected = 0;
            option = Option<int>.Some(4);
            actual = option
                .Filter(i => i > 0)
                .Filter(i => i % 2 > 0)
                .Match(
                    none => 0,
                    some => some.Value);

            Assert.Equal(expected, actual);
        }
    }
}