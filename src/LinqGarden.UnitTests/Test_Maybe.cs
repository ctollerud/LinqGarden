using FluentAssertions;
using System;
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
				() => "?" )
			.Should().Be( "?" );

		[Fact]
		public void To2_EmptyInput_ProducesExpectedResult() => 
			Maybe.None<int>().To<string>( 
				() => "?",
				x => x.ToString() )
			.Should().Be( "?" );

		[Fact]
		public void To_SomeInput_ProducesExpectedResult() =>
			Maybe.Some( 42 )
			.To<string>( 
				x => x.ToString(),
				() => "?" )
			.Should().Be( "42" );

		[Fact]
		public void To2_SomeInput_ProducesExpectedResult() =>
			Maybe.Some( 42 )
			.To<string>(
				() => "?",
				x => x.ToString() )
			.Should().Be( "42" );

		[Fact]
		public void Some_DoesNotAllowNullValues() =>
			( null as string )
			.Invoking( x => Maybe.Some( x ) )
			.Should().Throw<ArgumentNullException>();
	}
}
