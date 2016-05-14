using System;
using System.Collections.Generic;

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

        public TItem Resolve<TItem>()
        {
            var requestType = typeof (TItem);
            var resolvedType = RegistredItems[requestType];
            return (TItem)Activator.CreateInstance(resolvedType);
        }
    }
}
