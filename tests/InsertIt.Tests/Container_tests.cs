using Shouldly;
using Xunit;

namespace InsertIt.Tests
{
    // ReSharper disable once InconsistentNaming
    public class Container_tests
    {
        [Fact]
        public void should_register_properly()
        {
            var container = new Container(x =>
            {
                x.Record<ITestFirst>().As<Test>();
                x.Record<ITestSecond>().As<Test>();
            });
            var resolveFirst = container.Resolve<ITestFirst>();
            var resolveSecond = container.Resolve<ITestSecond>();
            resolveFirst.ShouldBeOfType<Test>();
            resolveSecond.ShouldBeOfType<Test>();
        }

    }

    internal interface ITestFirst
    {
    }

    internal class Test : ITestFirst, ITestSecond
    {
    }

    internal interface ITestSecond
    {
        
    }
}
