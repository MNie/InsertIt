namespace InsertIt.Common.Interfaces
{
    public interface IPossibleToAddCtors
    {
        IPossibleToAddCtors Ctor<TItem>(TItem value);
        IPossibleToAddCtors Ctor<TItem>(string argName, TItem value);
    }
}