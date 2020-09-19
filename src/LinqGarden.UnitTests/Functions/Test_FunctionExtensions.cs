using FluentAssertions;
using LinqGarden.Functions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace LinqGarden.UnitTests.Functions
{
	public static class Test_FunctionExtensions
	{
		[Fact]
		public static void Pipe_ProducesExpectedOutput() =>
			12345
			.Pipe( x => x.ToString() )
			.Should().Be( "12345" );

		[Fact]
		public static void Pipe_WhenFunctionIsNull_ArgumentNullExceptionIsThrown() =>
			12345
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            .Invoking( i => i.Pipe<int, string>( null ) )
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            .Should().Throw<ArgumentNullException>();
	}
}
