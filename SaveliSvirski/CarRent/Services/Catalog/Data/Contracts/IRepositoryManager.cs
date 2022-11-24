namespace Data.Contracts
{
    public interface IRepositoryManager
    {
        ICarMakeRepository Makes { get; }
        ICarModelRepository Models { get; }
        Task SaveAsync(CancellationToken cancellationToken = new());
    }
}