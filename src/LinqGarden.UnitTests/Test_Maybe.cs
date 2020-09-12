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

		[Fact]
		public void ValueOrDefault_WhenInputIsNoneString_ResultShouldBeNull() =>
			Maybe.None<string>()
			.ValueOrDefault()
			.Should().BeNull();

		[Fact]
		public void ValueOrDefault_WhenInputIsNoneInt_ResultShouldBeInputValue() =>
			Maybe.None<int>()
			.ValueOrDefault()
			.Should().Be( 0 );

		[Fact]
		public void ValueOrDefault_WhenInputIs42_ResultShouldBe42() =>
			Maybe.Some( 42 )
			.ValueOrDefault()
			.Should().Be( 42 );

		[Fact]
		public void ValueOrDefault_WhenInputIsSomeString_ResultShouldBeSameString() =>
			Maybe.Some( "abc" )
			.ValueOrDefault()
			.Should().Be( "abc" );
	}
}
