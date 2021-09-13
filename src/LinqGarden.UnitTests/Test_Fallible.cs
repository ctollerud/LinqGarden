using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace LinqGarden.UnitTests
{
	public class Test_Fallible
	{
		[Fact]
		public void WhenCallingSelect_AndInputIsError_ResultIsAlsoError()
		{
			var resultValue = 
				Fallible.Failure<string, int>( "oops" )
					.Select( x => x + 1 );
			resultValue.GetFailure().ValueOrDefault().Should().Be( "oops" );
			resultValue.GetSuccess().ValueOrDefault().Should().Be( default( int ) );
		}

		[Fact]
		public void WhenCallingSelect_AndInputIsSuccess_ResultIsAlsoSuccess()
		{
			var resultValue =
				Fallible.Success<string, int>( 42 )
					.Select( x => x + 1 );
			resultValue.GetFailure().ValueOrDefault().Should().BeNull();
			resultValue.GetSuccess().ValueOrDefault().Should().Be( 43 );
		}

		[Fact]
		public void WhenCallingSelectMany_AndInputIsFailure_ResultIsFailure()
		{
			var resultValue =
				Fallible.Failure<string, int>( "oops" )
					.SelectMany( x => Fallible.Success<string,int>( x + 1 ), (x,y) => (x + y).ToString() );
			resultValue.GetFailure().ValueOrDefault().Should().Be( "oops" );
			resultValue.GetSuccess().ValueOrDefault().Should().Be( null );
		}

		[Fact]
		public void WhenCallingSelectMany_AndInputIsSuccess_ButTransformationFails_ResultIsFailure()
		{
			var resultValue =
				Fallible.Success<string, int>( 42 )
					.SelectMany( x => Fallible.Failure<string, int>( "oops" ), ( x, y ) => ( x + y ).ToString() );
			resultValue.GetFailure().ValueOrDefault().Should().Be( "oops" );
			resultValue.GetSuccess().ValueOrDefault().Should().Be( default );
		}

		[Fact]
		public void WhenCallingSelectMany_AndInputIsSuccess_AndTransformationReturnsSuccess_ResultIsSuccess()
		{
			var resultValue =
				Fallible.Success<string, int>( 42 )
					.SelectMany( x => Fallible.Success<string, int>( x + 1 ), ( x, y ) => ( x + y ).ToString() );
			resultValue.GetFailure().ValueOrDefault().Should().Be( null );
			resultValue.GetSuccess().ValueOrDefault().Should().Be( (42+43).ToString() );
		}

		[Fact]
		public void IfNoneFail_WhenInputIsNone_ResultIsFailure()
		{
			var resultValue = Maybe.None<string>().IfNoneFail( "fail" );
			resultValue.GetFailure().ValueOrDefault().Should().Be( "fail" );
			resultValue.GetSuccess().ValueOrDefault().Should().BeNull();
		}

		[Fact]
		public void IfNoneFail_WhenInputIsSome_ResultIsSuccess()
		{
			var resultValue = Maybe.Some( 42 ).IfNoneFail( "fail" );
			resultValue.GetFailure().ValueOrDefault().Should().BeNull();
			resultValue.GetSuccess().ValueOrDefault().Should().Be( 42 );
		}
	}
}
