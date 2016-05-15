﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace InsertIt
{
    public class Container
    {
        private static readonly Dictionary<Type, Type> RegistredItems = new Dictionary<Type, Type>(); 
        public Container(Action<RegistredItem> action)
        {
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
            var ctors = resolvedType.GetTypeInfo().DeclaredConstructors;
            var dependencies = ctors?.First().GetParameters().Select(x => Resolve(x.ParameterType)).ToArray();

            return Activator.CreateInstance(resolvedType, dependencies);
        }

        public TItem Resolve<TItem>()
        {
            return (TItem)Resolve(typeof(TItem));
        }
    }
}
