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
		public static void AsCollection_WhenInputIsNull_NullArgumentExceptionGetsReturned() =>
			( null as int[] )
			.Invoking( x => x.AsCollection() ).Should().Throw<ArgumentNullException>();

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
	}
}
