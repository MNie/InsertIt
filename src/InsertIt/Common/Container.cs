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
        protected static Dictionary<Type, Dictionary<Type, object>> CtorDictionary; 
        
        public Container(Action<RegistredItem> action)
        {
            RegistredItems = new Dictionary<Type, Type>();
            CtorDictionary = new Dictionary<Type, Dictionary<Type, object>>();
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
                CtorDictionary.Add(Register, null);
                return this;
            }

            public ConcreteCtor<TItem> Ctor<TItem>()
            {
                return new ConcreteCtor<TItem>(Register, typeof(TItem), this);
            }
        }

        public class ConcreteCtor<TItem>
        {
            public Type Ctor { get; set; }
            public TItem Value { get; set; }
            private readonly RegistredItem _this;
            private readonly Type _key;

            public ConcreteCtor(Type key, Type ctor, RegistredItem _this)
            {
                Ctor = ctor;
                this._this = _this;
                _key = key;
            }

            public RegistredItem Is(object value)
            {
                var values = CtorDictionary[_key] ?? new Dictionary<Type, object>();
                values.Add(typeof(TItem), (TItem)value);
                CtorDictionary[_key] = values;
                return _this;
            }
        }


        private static object Resolve(Type requestType)
        {
            var resolvedType = RegistredItems[requestType];
            var ctors = resolvedType.GetTypeInfo().DeclaredConstructors.ToList();
            if(ctors.Count() > 1)
                throw new ClassHasMultipleConstructorsException(requestType);
            var ctorParameters = CtorDictionary[requestType];
            var dependencies = ctors?
                .First()
                .GetParameters()
                .Select(x =>
                {
                    if (ctorParameters == null) return Resolve(x.ParameterType);
                    return ctorParameters.ContainsKey(x.ParameterType) ? 
                        ctorParameters[x.ParameterType] : 
                        Resolve(x.ParameterType);
                })
                .ToArray();

            return Activator.CreateInstance(resolvedType, dependencies);
        }

        public TItem Resolve<TItem>()
        {
            return (TItem)Resolve(typeof(TItem));
        }
    }

    
}
