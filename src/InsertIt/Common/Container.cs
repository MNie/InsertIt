﻿using System;
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
            private Type _register;
            private Type _qua;

            public RegistredItem Record<TItem>()
            {
                _register = typeof(TItem);
                return this;
            }

            public RegistredItem As<TItem>()
            {
                _qua = typeof (TItem);
                RegistredItems.Add(_register, _qua);
                CtorDictionary.Add(_register, null);
                return this;
            }

            public RegistredItem Ctor<TItem>(TItem value)
            {
                var values = CtorDictionary[_register] ?? new Dictionary<Type, object>();
                values.Add(typeof(TItem), value);
                CtorDictionary[_register] = values;
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
