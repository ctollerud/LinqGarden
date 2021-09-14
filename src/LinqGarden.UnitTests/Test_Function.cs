using System;
using System.Collections.Generic;
using System.IO;
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

            Function.From(() =>
            {
                if (true)
                {
                    throw expectedThrownException;
                }
            })
            .CatchAsFailure<InvalidOperationException>().Invoke()
            .GetFailure().Should().Be(expectedThrownException.NoneIfNull());
        }

        [Fact]
        public void MultipleExceptionsAreToBeCaught_ExpectedExceptionIsCaught()
        {
            var expectedThrownException = new InvalidOperationException("Oh no") as Exception;

            Function.From(() =>
            {
                if (true)
                {
                    throw expectedThrownException;
                }
            })
            .CatchAsFailure<InvalidOperationException>()
            .CatchAsFailure<DirectoryNotFoundException>()
            .Invoke()
            .GetFailure().Should().Be(expectedThrownException.NoneIfNull());
        }

        [Fact]
        public void MultipleExceptionsAreToBeCaught_ExpectedExceptionIsCaught2()
        {
            var expectedThrownException = new DirectoryNotFoundException("Oh no") as Exception;

            Function.From(() =>
            {
                if (true)
                {
                    throw expectedThrownException;
                }
            })
            .CatchAsFailure<InvalidOperationException>()
            .CatchAsFailure<DirectoryNotFoundException>()
            .CatchAsFailure<Exception>()
            .Invoke()
            .GetFailure().Should().Be(expectedThrownException.NoneIfNull());
        }



        [Fact]
        public void WhenFailingToCatchException_ExceptionIsThrown()
        {
            var expectedThrownException = new Exception("Oh no");
            Assert.Throws<Exception>(() =>
            {
                Function.From(() =>
                {
                    if (true)
                    {
                        throw expectedThrownException;
                    }
                })
                .CatchAsFailure<InvalidOperationException>().Invoke();
            });
        }

        [Fact]
        public void WhenFunctionSucceeds_ExpectedValueGetsReturned()
        {
            Function.From<int,int>(x => x+2).CatchAsFailure<Exception>().Invoke(2).GetSuccess()
                .Should().Be(4.NoneIfNull());
        }
    }
}
