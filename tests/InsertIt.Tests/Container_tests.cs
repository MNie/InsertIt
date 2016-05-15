using InsertIt.Exceptions;
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

        [Fact]
        public void should_register_properly_with_dependencies()
        {
            var container = new Container(x =>
            {
                x.Record<Dependency>().As<Dependency>();
                x.Record<ITestWithDependency>().As<TestWithDependency>();
            });
            var resolve = container.Resolve<ITestWithDependency>();
            resolve.ShouldBeOfType<TestWithDependency>();
        }

        [Fact]
        public void should_fails_while_register_class_with_multiple_ctors()
        {
            var container = new Container(x =>
            {
                x.Record<TestWithMultipleDependency>().As<TestWithMultipleDependency>();
            });
            var exc = Record.Exception(() => container.Resolve<TestWithMultipleDependency>());
            exc.ShouldNotBeNull();
            exc.ShouldBeOfType<ClassHasMultipleConstructorsException>();
        }
    }

    

    internal interface ITestFirst {}
    internal interface ITestSecond {}
    internal interface ITestWithDependency { }
    internal class Test : ITestFirst, ITestSecond {}

    internal class Dependency {}

    internal class TestWithDependency : ITestWithDependency
    {
        public TestWithDependency(Dependency dependency) {}
    }

    internal class TestWithMultipleDependency : ITestWithDependency
    {
        public TestWithMultipleDependency(Dependency dependency, Dependency dependencySecond){ }
        public TestWithMultipleDependency(Dependency dependency) { }
    }
}
