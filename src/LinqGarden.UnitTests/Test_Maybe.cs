using FluentAssertions;
using System;
using System.Text;
using Xunit;

namespace LinqGarden.UnitTests
{
    public class Test_Maybe
    {
        [Fact]
        public void To_EmptyInput_ProducesExpectedResult() =>
            Maybe.None<int>()
            .To<string>(
                x => x.ToString(),
                () => "?")
            .Should().Be("?");

        [Fact]
        public void To2_EmptyInput_ProducesExpectedResult() =>
            Maybe.None<int>().To<string>(
                () => "?",
                x => x.ToString())
            .Should().Be("?");

        [Fact]
        public void To_SomeInput_ProducesExpectedResult() =>
            Maybe.Some(42)
            .To<string>(
                x => x.ToString(),
                () => "?")
            .Should().Be("42");

        [Fact]
        public void To2_SomeInput_ProducesExpectedResult() =>
            Maybe.Some(42)
            .To<string>(
                () => "?",
                x => x.ToString())
            .Should().Be("42");

        [Fact]
        public void Some_DoesNotAllowNullValues() =>
            (null as string)
            .Invoking(x => Maybe.Some(x))
            .Should().Throw<ArgumentNullException>();

        [Fact]
        public void ValueOrDefault_WhenInputIsNoneString_ResultShouldBeNull() =>
            Maybe.None<string>()
            .ValueOrDefault()
            .Should().BeNull();

        [Fact]
        public void ValueOrDefault_WhenInputIsNoneInt_ResultShouldBeInputValue() =>
            Maybe.None<int>()
            .ValueOrDefault()
            .Should().Be(0);

        [Fact]
        public void ValueOrDefault_WhenInputIs42_ResultShouldBe42() =>
            Maybe.Some(42)
            .ValueOrDefault()
            .Should().Be(42);

        [Fact]
        public void ValueOrDefault_WhenInputIsSomeString_ResultShouldBeSameString() =>
            Maybe.Some("abc")
            .ValueOrDefault()
            .Should().Be("abc");

        private void VerifyEquals<T>(Maybe<T> first, Maybe<T> second)
        {
            first.Should().Be(second);
            (first == second).Should().BeTrue();
            (first != second).Should().BeFalse();

        }

        private void VerifyNotEquals<T>(Maybe<T> first, Maybe<T> second)
        {
            first.Should().NotBe(second);
            (first == second).Should().BeFalse();
            (first != second).Should().BeTrue();
        }

        [Fact]
        public void DefaultMaybeEqualsDefaultMaybe() =>
            VerifyEquals(default(Maybe<string>), default(Maybe<string>));

        [Fact]
        public void DefaultMaybeEqualsNoneMaybe() =>
            VerifyEquals(default(Maybe<string>), Maybe.None<string>());

        [Fact]
        public void NoneMaybeEqualsDefaultMaybe() =>
            VerifyEquals(Maybe.None<string>(), default(Maybe<string>));

        [Fact]
        public void SomeDoesNotEqualNone() =>
            VerifyNotEquals(Maybe.Some(42), Maybe.None<int>());

        [Fact]
        public void NoneDoesNotEqualSome() =>
            VerifyNotEquals(Maybe.None<int>(), Maybe.Some(42));

        [Fact]
        public void Some42DoesNotEqualSome43() =>
            VerifyNotEquals(Maybe.Some(42), Maybe.Some(43));

        [Fact]
        public void Some42EqualsSome42() =>
            VerifyEquals(Maybe.Some(42), Maybe.Some(42));

        [Fact]
        public void IdentitalValuesProduceIdenticalHashCodes() =>
            Maybe.Some(42).GetHashCode().Should().Be(Maybe.Some(42).GetHashCode());

        [Fact]
        public void MaybesOfDifferentTypesAreNeverEqual() =>
            Maybe.Some(42).Should().NotBe(Maybe.Some((object)42));

        [Fact]
        public void ToMaybe_WhenInputIsNull_ResultIsNone() =>
            (null as StringBuilder).ToMaybe().Should().Be(Maybe.None<StringBuilder>());

        [Fact]
        public void ToMaybe_WhenInputHasValue_ResultHasValue()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.ToMaybe().Should().Be(Maybe.Some(stringBuilder));
        }

        private class Incrementor
        {
            public bool WasCalled { get; private set; } = false;

            public int Increment(int input)
            {
                WasCalled = true;
                return input + 1;
            }
        }

        [Fact]
        public void Select_WhenInputHasValue_TransformationIsPerformed()
        {
            var incrementor = new Incrementor();

            42.ToMaybe().Select(incrementor.Increment).ValueOrDefault().Should().Be(43);

            incrementor.WasCalled.Should().BeTrue();
        }

        [Fact]
        public void Select_WhenInputHasNoValue_TransformationIsNotPerformed()
        {
            var incrementor = new Incrementor();

            Maybe.None<int>().Select(x => x + 1).ValueOrDefault().Should().Be(0);

            incrementor.WasCalled.Should().BeFalse();
        }

        [Fact]
        public void SelectMany_WhenInputHasNoValue_TransformationIsNotPerformed()
        {
            var firstIncrementor = new Incrementor();
            var secondIncrementor = new Incrementor();

            Maybe.None<int>()
                .SelectMany(
                    x => firstIncrementor.Increment(x).ToMaybe(),
                    (x, y) => secondIncrementor.Increment(x + y))
            .Should().Be(Maybe.None<int>());

            firstIncrementor.WasCalled.Should().BeFalse();
            secondIncrementor.WasCalled.Should().BeFalse();
        }

        [Fact]
        public void SelectMany_WhenTransformationFails_CombinerIsNotPerformed()
        {
            var firstIncrementor = new Incrementor();
            var secondIncrementor = new Incrementor();

            42.ToMaybe()
                .SelectMany(
                    x => firstIncrementor.Increment(x).ToMaybe().Where(x => x != 43),
                    (x, y) => secondIncrementor.Increment(x + y))
           .Should().Be(Maybe.None<int>());

            firstIncrementor.WasCalled.Should().BeTrue();
            secondIncrementor.WasCalled.Should().BeFalse();
        }

        [Fact]
        public void SelectMany_WhenTransformationSucceeds_EverythingGetsCalled()
        {
            var firstIncrementor = new Incrementor();
            var secondIncrementor = new Incrementor();

            42.ToMaybe()
                .SelectMany(
                    x => firstIncrementor.Increment(x).ToMaybe(),
                    (x, y) => secondIncrementor.Increment(x + y))
            .Should().Be((42 + 43 + 1).ToMaybe());

            firstIncrementor.WasCalled.Should().BeTrue();
            secondIncrementor.WasCalled.Should().BeTrue();
        }

        [Fact]
        public void SelectMany_WithQuerySyntax_WhenInputHasNoValue_TransformationIsNotPerformed()
        {
            var firstIncrementor = new Incrementor();
            var secondIncrementor = new Incrementor();

            (from x in Maybe.None<int>()
             from incremented in firstIncrementor.Increment(x).ToMaybe()
             select x + incremented)
            .Should().Be(Maybe.None<int>());

            firstIncrementor.WasCalled.Should().BeFalse();
            secondIncrementor.WasCalled.Should().BeFalse();
        }

        [Fact]
        public void SelectMany_WithQuerySyntax_WhenTransformationFails_CombinerIsNotPerformed()
        {
            var firstIncrementor = new Incrementor();
            var secondIncrementor = new Incrementor();

            (from x in 42.ToMaybe()
             from incremented in firstIncrementor.Increment(x).ToMaybe().Where(x => x != 43)
             select secondIncrementor.Increment(x + incremented))
            .Should().Be(Maybe.None<int>());

            firstIncrementor.WasCalled.Should().BeTrue();
            secondIncrementor.WasCalled.Should().BeFalse();
        }

        [Fact]
        public void SelectMany_WithQuerySyntax_WhenTransformationSucceeds_EverythingGetsCalled()
        {
            var firstIncrementor = new Incrementor();
            var secondIncrementor = new Incrementor();

            (from x in 42.ToMaybe()
             from incremented in firstIncrementor.Increment(x).ToMaybe()
             select secondIncrementor.Increment(x + incremented))
            .Should().Be(
                ((42 + 43) + 1).ToMaybe());

            firstIncrementor.WasCalled.Should().BeTrue();
            secondIncrementor.WasCalled.Should().BeTrue();
        }

        [Fact]
        public void Where_WhenInputIsNone_ResultIsNone() =>
            Maybe.None<int>().Where(x => x != 42).Should().Be(Maybe.None<int>());

        [Fact]
        public void Where_WhenFilterReturnsFalse_ResultIsNone() =>
            Maybe.Some(42).Where(x => x != 42).Should().Be(Maybe.None<int>());

        [Fact]
        public void Where_WhenFilterReturnsTrue_ResultIsNone() =>
            Maybe.Some(42).Where(x => x == 42).Should().Be(42.ToMaybe());
    }
}
