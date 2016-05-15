using System;

namespace InsertIt.Exceptions
{
    public class ClassHasMultipleConstructorsException : ClassException
    {
        public ClassHasMultipleConstructorsException(Type type) : base(type)
        {
        }
    }
}
