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
        [InlineData(5, "above two")]
        [InlineData(1, "below or equal to two")]
        public void Bind_to_different_data_type(int value, string expected)
        {
            Option<int> option = value;

            var actual = option
                .Bind(i => i > 2
                    ? Option<string>.Some("above two")
                    : Option<string>.None())
                .Match(
                    none => "below or equal to two",
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

        [Fact]
        public void Use_Do_to_execute_conditional_actions()
        {
            Option<int> option = 5;

            option
                .DoIfNone(() => Assert.True(false, "this should not be executed"))
                .Do(i => Assert.Equal(5, i));
        }

        [Fact]
        public void Use_DoIfNone_to_execute_conditional_actions()
        {
            Option<int> option = new None();

            option
                .Do(i => Assert.True(false, "this should not be executed"))
                .DoIfNone(() => Assert.True(true, "this should be executed"));
        }

        [Fact]
        public void ToOption_should_return_Some_for_value_types()
        {
            int i = default;
            i.ToOption()
                .Switch(
                    none => Assert.True(false, "should not be None"),
                    some => Assert.True(true, "should be Some<int>"));

            i = 5;
            i.ToOption()
                .Switch(
                    none => Assert.True(false, "should not be None"),
                    some => Assert.True(true, "should be Some<int>"));
        }

        [Fact]
        public void ToOption_should_return_Some_or_None_for_reference_types()
        {
            TestClass t = default;
            t.ToOption()
                .Switch(
                    none => Assert.True(true, "should be None"),
                    some => Assert.True(false, "should not be Some<TestClass>"));

            t = new TestClass();
            t.ToOption()
                .Switch(
                    none => Assert.True(false, "should not be None"),
                    some => Assert.True(true, "should be Some<TestClass>"));
        }

        class TestClass
        {

        }
    }
}