using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using InsertIt.Common.Interfaces;
using InsertIt.Exceptions;

namespace InsertIt.Common
{
    public class Container
    {
        protected static Dictionary<Type, Type> RegistredItems;
        protected static Dictionary<Type, Dictionary<Type, object>> CtorDictionary;
        protected static Dictionary<Type, Dictionary<string, object>> ReflectedCtorDictionary;

        public Container(Action<IWantToRegisterItem> action)
        {
            RegistredItems = new Dictionary<Type, Type>();
            CtorDictionary = new Dictionary<Type, Dictionary<Type, object>>();
            ReflectedCtorDictionary = new Dictionary<Type, Dictionary<string, object>>();
            action.Invoke(new RegistredItem());
        }

        public class RegistredItem : IWantToRegisterItem, INotYetRegistred, IPossibleToAddCtors
        {
            private Type _register;
            private Type _qua;

            public INotYetRegistred Record<TItem>()
            {
                _register = typeof(TItem);
                return this;
            }

            public IPossibleToAddCtors As<TItem>()
            {
                _qua = typeof (TItem);
                RegistredItems.Add(_register, _qua);
                CtorDictionary.Add(_register, null);
                ReflectedCtorDictionary.Add(_register, null);
                return this;
            }

            public IPossibleToAddCtors Ctor<TItem>(TItem value)
            {
                var values = CtorDictionary[_register] ?? new Dictionary<Type, object>();
                values.Add(typeof(TItem), value);
                CtorDictionary[_register] = values;
                return this;
            }

            public IPossibleToAddCtors Ctor<TItem>(string argName, TItem value)
            {
                var values = ReflectedCtorDictionary[_register] ?? new Dictionary<string, object>();
                values.Add(argName, value);
                ReflectedCtorDictionary[_register] = values;
                return this;
            }
        }

        private static object Resolve(Type requestType)
        {
            var resolvedType = RegistredItems[requestType];
            var ctors = resolvedType.GetTypeInfo().DeclaredConstructors.ToList();
            if(ctors.Count() > 1)
                throw new ClassHasMultipleConstructorsException(requestType);
            var ctorParameters = CtorDictionary[requestType];
            var reflectedCtorParameters = ReflectedCtorDictionary[requestType];
            var dependencies = ctors?
                .First()
                .GetParameters()
                .Select(x => ctorParameters == null && reflectedCtorParameters == null ? Resolve(x.ParameterType) : ResolveIfItsSetByCtor(ctorParameters, reflectedCtorParameters, x))
                .ToArray();

            return Activator.CreateInstance(resolvedType, dependencies);
        }

        private static object ResolveIfItsSetByCtor(IReadOnlyDictionary<Type, object> ctorParameters, IReadOnlyDictionary<string, object> reflectedCtorParameters, ParameterInfo x)
        {
            if(reflectedCtorParameters != null)
                if (reflectedCtorParameters.ContainsKey(x.Name))
                    return reflectedCtorParameters[x.Name];
            if(ctorParameters != null)
                if (ctorParameters.ContainsKey(x.ParameterType))
                    return ctorParameters[x.ParameterType];
            return Resolve(x.ParameterType);
        }

        public TItem Resolve<TItem>()
        {
            return (TItem)Resolve(typeof(TItem));
        }
    }
}