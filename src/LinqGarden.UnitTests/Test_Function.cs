using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Xunit;

namespace LinqGarden.UnitTests
{
    public class Test_Function
    {
        [Fact]
        public void WhenCatchingExceptionAsFailure_ExpectedBehaviorOccurs()
        {
            var expectedThrownException = new InvalidOperationException("Oh no");

            Function.FromAction(() =>
            {
                if (true)
                {
                    throw expectedThrownException;
                }
            })
            .CatchAsFailure<InvalidOperationException>().Invoke()
            .GetFailure().Should().Be(expectedThrownException.ToMaybe());
        }

        [Fact]
        public void WhenFailingToCatchException_ExceptionIsThrown()
        {
            var expectedThrownException = new Exception("Oh no");
            Assert.Throws<Exception>(() =>
            {
                Function.FromAction(() =>
                {
                    if (true)
                    {
                        throw expectedThrownException;
                    }
                })
                .Catch<InvalidOperationException>().Invoke();
            });
        }

        [Fact]
        public void WhenFunctionSucceeds_ExpectedValueGetsReturned()
        {
            FallibleFunction.Build(() => 42).Catch<Exception>().Invoke().GetSuccess()
                .Should().Be(42.ToMaybe());
        }
    }
}
