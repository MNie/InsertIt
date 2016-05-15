using System;

namespace InsertIt.Exceptions
{
    public abstract class ClassException : Exception
    {
        public readonly Type Type;

        protected ClassException(Type type)
        {
            Type = type;
        }
    }
}
