<b>InsertIt</b> is a simple Dependency Injection library for Windows Phone.

How to install <b>InsertIt</b>?</br>
<b>InsertIt</b> is a nuget package (<i>https://www.nuget.org/packages/InsertIt/</i>), so You can simply type following command in a  Package Manager Console:

    Install-Package InsertIt

What You can do?
Register simple Dependency:
    
    _container = new Container(x =>
            {
                x.Record<IInterface>().As<ClassToResolve>();
            });
Register dependency with dependency inside:

    _container = new Container(x =>
            {
                x.Record<Dependency>().As<Dependency>();
                x.Record<IInterface>().As<ClassWithDependency>();
            });

Register dependency with value for constructor arguments:

    _container = new Container(x =>
            {
                x.Record<IInterface>().As<ClassWithCtorWithArgs>().Ctor("test");
            });
            
How to get resolved dependecy?

    var resolve = _container.Resolve<IInterface>();
    
How to make it works in Your windows phone app?
Create static class:

    public static class Registry
    {
        private static Container _container;

        public static void Initialize()
        {
            _container = new Container(_ =>
            {
                _.Record<IInterface>().As<ClassToResolve>();
            });
        }

        public static TItem Get<TItem>()
        {
            return _container.Resolve<TItem>();
        }
    }
    
And invoke <i>Initialize</i> method in <i>OnLaunched</i> method in <i>App.xaml.cs</i>

    protected override void OnLaunched(LaunchActivatedEventArgs e)
    {
        ...
        Registry.Initialize();
        ...
    }
