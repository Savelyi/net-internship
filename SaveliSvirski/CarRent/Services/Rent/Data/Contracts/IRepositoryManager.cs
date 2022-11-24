namespace Data.Contracts
{
    public interface IRepositoryManager
    {
        IRentRepository Rents { get; }
        ICarRepository Cars { get; }

        Task SaveAsync(CancellationToken cancellationToken = new CancellationToken());
        void Save();
    }
}