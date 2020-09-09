using FluentAssertions;
using LinqGarden.Enumerables;
using System;
using System.Collections.Generic;
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
	}
}
