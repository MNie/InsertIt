using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using InsertIt.Exceptions;

namespace InsertIt
{
    public class Container
    {
        protected static Dictionary<Type, Type> RegistredItems; 
        public Container(Action<RegistredItem> action)
        {
            RegistredItems = new Dictionary<Type, Type>();
            action.Invoke(new RegistredItem());
        }

        public class RegistredItem
        {
            public Type Register { get; set; }
            public Type Qua { get; set; }

            public RegistredItem Record<TItem>()
            {
                Register = typeof(TItem);
                return this;
            }

            public RegistredItem As<TItem>()
            {
                Qua = typeof (TItem);
                RegistredItems.Add(Register, Qua);
                return this;
            }
        }

        private static object Resolve(Type requestType)
        {
            var resolvedType = RegistredItems[requestType];
            var ctors = resolvedType.GetTypeInfo().DeclaredConstructors.ToList();
            if(ctors.Count() > 1)
                throw new ClassHasMultipleConstructorsException(requestType);
            var dependencies = ctors?.First().GetParameters().Select(x => Resolve(x.ParameterType)).ToArray();

            return Activator.CreateInstance(resolvedType, dependencies);
        }

        public TItem Resolve<TItem>()
        {
            return (TItem)Resolve(typeof(TItem));
        }
    }
}
