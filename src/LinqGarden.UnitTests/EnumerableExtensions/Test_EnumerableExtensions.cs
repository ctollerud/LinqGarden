using FluentAssertions;
using LinqGarden.Enumerables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace LinqGarden.UnitTests.EnumerableExtensions
{
	public class Test_EnumerableExtensions
	{
		[Fact]
		public static void StartWith_ReturnsExpectedResults() =>
			new[] { 42, 43, 44 }
			.StartWith( 1337 )
			.Should().BeEquivalentTo( 
				new[] { 1337, 42, 43, 44 },
				o => o.WithStrictOrdering() );

		[Fact]
		public static void AsCollection_WhenInputIsNotACollection_ExpectedResultReturned() =>
			Enumerable.Range( 1, 3 )
			.AsCollection().Should().BeEquivalentTo( new[] { 1, 2, 3 }, o => o.WithStrictOrdering() );

		[Fact]
		public static void AsCollection_WhenInputIsAlreadyCollection_InputCollectionIsReturned()
		{
			var input = new[] { 1, 2, 3 };
			input.AsCollection().Should().BeSameAs( input );
		}

        [Fact]
        public static void Do_DoesNotPerformWork_UntilForEachCalled()
        {
            int sum = 0;
            IEnumerable<int> enumerable =
            new[]{ 1,2,3 }
            .Do(x =>
            {
                sum += x;
            });

            sum.Should().Be(0, "Do is lazy, so no work should have been performed yet.");

            enumerable.ForEach();

            sum.Should().Be(6, "The sequence has been evaluated so everything should have been added up.");
        }

        [Fact]
        public static void FirstOrNone_WhenSequenceIsEmpty_ResultIsNone() =>
            new string[] { }.FirstOrNone().Should().Be(Maybe.None<string>());

        [Fact]
        public static void FirstOrNone_WhenFirstValueIsNull_ResultIsNone() =>
            new string?[] { null, "abc" }.FirstOrNone().Should().Be(Maybe.None<string>());

        [Fact]
        public static void FirstOrNone_WhenFirstValueIsSet_ResultIsValue() =>
            new string?[] { "abc", "def" }.FirstOrNone().Should().Be(Maybe.Some( "abc" ));
    }
}
