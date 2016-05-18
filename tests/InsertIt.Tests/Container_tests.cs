using InsertIt.Common;
using InsertIt.Exceptions;
using Shouldly;
using Xunit;

namespace InsertIt.Tests
{
    // ReSharper disable once InconsistentNaming
    public class Container_tests
    {
        private Container _container;

        private ITest Act()
        {
            return _container.Resolve<ITest>();
        }
        [Fact]
        public void should_register_properly()
        {
            _container = new Container(x =>
            {
                x.Record<ITest>().As<Test>();
                x.Record<ITestSecond>().As<Test>();
            });
            var resolveFirst = Act();
            var resolveSecond = _container.Resolve<ITestSecond>();
            resolveFirst.ShouldBeOfType<Test>();
            resolveSecond.ShouldBeOfType<Test>();
        }

        [Fact]
        public void should_register_properly_with_dependencies()
        {
            _container = new Container(x =>
            {
                x.Record<Dependency>().As<Dependency>();
                x.Record<ITest>().As<TestWithSingleDependency>();
            });
            var resolve = Act();
            resolve.ShouldBeOfType<TestWithSingleDependency>();
        }

        [Fact]
        public void should_fails_while_register_class_with_multiple_ctors()
        {
            _container = new Container(x =>
            {
                x.Record<ITest>().As<TestWithMultipleDependency>();
            });
            var exc = Record.Exception(() => Act());
            exc.ShouldNotBeNull();
            exc.ShouldBeOfType<ClassHasMultipleConstructorsException>();
        }

        [Fact]
        public void should_register_properly_with_concrete_dependencies()
        {
            _container = new Container(x =>
            {
                x.Record<ITest>().As<TestWithConcreteDependency>().Ctor("test");
            });
            var resolve = Act();
            resolve.ShouldBeOfType<TestWithConcreteDependency>();
            ((TestWithConcreteDependency)resolve).Name.ShouldBe("test");
        }

        [Fact]
        public void should_register_properly_with_typed_class()
        {
            _container = new Container(x =>
            {
                x.Record<ITypedTest<Test>>().As<TypedTest<Test>>();
            });
            var resolve = _container.Resolve<ITypedTest<Test>>();
            resolve.ShouldBeOfType<TypedTest<Test>>();
        }
    }

    

    internal interface ITest {}
    internal interface ITestSecond {}
    internal class Test : ITest, ITestSecond {}

    internal class Dependency {}

    internal class TestWithSingleDependency : ITest
    {
        public TestWithSingleDependency(Dependency dependency) {}
    }

    internal class TestWithMultipleDependency : ITest
    {
        public TestWithMultipleDependency(Dependency dependency, Dependency dependencySecond){ }
        public TestWithMultipleDependency(Dependency dependency) { }
    }

    internal class TestWithConcreteDependency : ITest
    {
        public string Name { get; set; }

        public TestWithConcreteDependency(string name)
        {
            Name = name;
        }
    }

    internal interface ITypedTest<TItem> { }
    internal class TypedTest<TItem> : ITypedTest<TItem> { }
}
