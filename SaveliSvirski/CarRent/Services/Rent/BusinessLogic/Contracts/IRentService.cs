using BusinessLogic.Dto;

namespace BusinessLogic.Contracts
{
    public interface IRentService
    {
        Task<IEnumerable<RentToShowDto>> ShowUserRentsAsync(Guid userId, CancellationToken cancellationToken);
        Task<RentToShowDto> ShowUserRentAsync(Guid rentId, Guid userId, CancellationToken cancellationToken);
        Task<Guid> CreateRentAsync(Guid carId, Guid userId, CancellationToken cancellationToken);
        Task CloseRentAsync(Guid rentId, Guid userId, CancellationToken cancellationToken);
        Task DeleteRentAsync(Guid rentId, Guid userId, CancellationToken cancellationToken);
        Task DeleteRentForAdminAsync(Guid rentId, CancellationToken cancellationToken);
    }
}