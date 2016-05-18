namespace InsertIt.Common.Interfaces
{
    public interface IWantToRegisterItem
    {
        INotYetRegistred Record<TItem>();
    }
}