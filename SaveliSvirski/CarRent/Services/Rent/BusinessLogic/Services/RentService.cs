using System.Net;
using AutoMapper;
using BusinessLogic.Contracts;
using BusinessLogic.Dto;
using Data.Contracts;
using Data.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using SharedModels.Cache;
using SharedModels.ErrorModels;
using SharedModels.RentEvents;

namespace BusinessLogic.Services
{
    public class RentService : IRentService
    {
        private readonly IMapper mapper;
        private readonly IRepositoryManager repository;
        private readonly IBus bus;
        private readonly ILogger<RentService> logger;
        private readonly IDistributedCache cache;
        private readonly IOptions<RedisOptions> redisOptions;

        public RentService(IMapper mapper, IRepositoryManager repository, IBus bus,
            ILogger<RentService> logger, IDistributedCache cache, IOptions<RedisOptions> redisOptions)
        {
            this.mapper = mapper;
            this.repository = repository;
            this.bus = bus;
            this.logger = logger;
            this.cache = cache;
            this.redisOptions = redisOptions;
        }

        public async Task<IEnumerable<RentToShowDto>> ShowUserRentsAsync(Guid userId,
            CancellationToken cancellationToken)
        {
            if (!cache.TryGetValue(userId.ToString(), out IEnumerable<RentToShowDto> rentsToShowDto))
            {
                var result = await repository.Rents.GetByCondition(e => e.UserId == userId, true)
                    .ToListAsync(cancellationToken);
                rentsToShowDto = mapper.Map<IEnumerable<RentToShowDto>>(result);
                await cache.SetAsync(userId.ToString(), rentsToShowDto);
            }

            return rentsToShowDto;
        }

        public async Task<RentToShowDto> ShowUserRentAsync(Guid rentId, Guid userId,
            CancellationToken cancellationToken)
        {
            var result = await CheckRentForIssuesAsync(rentId, userId, cancellationToken);
            return mapper.Map<RentToShowDto>(result);
        }

        public async Task<Guid> CreateRentAsync(Guid carId, Guid userId, CancellationToken cancellationToken)
        {
            var carResult = await repository.Cars.GetByIdAsync(carId, cancellationToken);
            if (carResult == null || carResult.IsAvailable == false)
            {
                throw new NotFoundException($"The car with Id {carId} was not found or has been already rented");
            }

            carResult.IsAvailable = false;
            var rent = new Rent()
            {
                CarId = carId,
                UserId = userId
            };
            repository.Cars.Update(carResult);
            await repository.Rents.CreateAsync(rent, cancellationToken);
            await repository.SaveAsync(cancellationToken);
            await cache.RemoveAsync(userId.ToString());

            await bus.Publish<ICarRentedEvent>(new CarRentedEvent
            {
                Id = carId
            }, cancellationToken);
            return rent.Id;
        }

        public async Task CloseRentAsync(Guid rentId, Guid userId, CancellationToken cancellationToken)
        {
            var result = await CheckRentForIssuesAsync(rentId, userId, cancellationToken, true);
            if (result.IsClosed)
            {
                throw new RentIssuesException($"The rent with Id {rentId} has already been closed");
            }

            result.Closed = DateTime.Now;
            result.IsClosed = true;
            result.CarInfo.IsAvailable = true;
            repository.Rents.Update(result);
            await repository.SaveAsync(cancellationToken);
            await cache.RemoveAsync(userId.ToString(), cancellationToken);

            await bus.Publish<ICarRentClosedEvent>(new CarRentClosedEvent
            {
                Id = result.CarId
            }, cancellationToken);
        }

        public async Task DeleteRentAsync(Guid rentId, Guid userId, CancellationToken cancellationToken)
        {
            var result = await CheckRentForIssuesAsync(rentId, userId, cancellationToken);
            if (result.IsClosed == false)
            {
                throw new RentIssuesException(
                    $"The rent with Id {rentId} has not been closed yet, so you can't delete it");
            }

            repository.Rents.Delete(result);
            await repository.SaveAsync(cancellationToken);

            await cache.RemoveAsync(userId.ToString(), cancellationToken);
        }

        public async Task DeleteRentForAdminAsync(Guid rentId, CancellationToken cancellationToken)
        {
            var result = await repository.Rents.GetByIdAsync(rentId, cancellationToken);
            if (result == null)
            {
                throw new NotFoundException($"The rent with Id {rentId} was not found");
            }

            if (result.IsClosed == false)
            {
                throw new RentIssuesException(
                    $"The rent with Id {rentId} has not been closed yet, so you can't delete it");
            }

            repository.Rents.Delete(result);
            await repository.SaveAsync(cancellationToken);

            await cache.RemoveAsync(result.UserId.ToString(), cancellationToken);
        }

        public async Task DoSomeProcessWithLockAsync()
        {
            var redOpt = redisOptions.Value;
            var endPoints = new List<RedLockEndPoint>
            {
                new DnsEndPoint(redOpt.Host, redOpt.Port)
            };
            var redlockFactory = RedLockFactory.Create(endPoints);
            var resource = "resource_to_lock_on";
            var expiry = TimeSpan.FromSeconds(3);
            var wait = TimeSpan.FromSeconds(1);
            var retry = TimeSpan.FromSeconds(1);
            using (var redLock = await redlockFactory.CreateLockAsync(resource, expiry, wait, retry))
            {
                if (redLock.IsAcquired)
                {
                    Thread.Sleep(1000); //do stuff
                }
            } // the lock is automatically released at the end of the using block
        }

        private async Task<Rent> CheckRentForIssuesAsync(Guid rentId, Guid userId, CancellationToken cancellationToken,
            bool trackChanges = false)
        {
            var rent = await repository.Rents.GetByIdAsync(rentId, cancellationToken, trackChanges);
            if (rent == null || rent.UserId != userId)
            {
                throw new NotFoundException($"The rent with Id {rentId} was not found");
            }

            return rent;
        }
    }
}