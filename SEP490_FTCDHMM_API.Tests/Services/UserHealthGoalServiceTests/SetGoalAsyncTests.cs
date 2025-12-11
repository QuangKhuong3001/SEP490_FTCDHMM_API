using System.Linq.Expressions;
using Moq;
using SEP490_FTCDHMM_API.Application.Dtos.UserHealthGoalDtos;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;
using SEP490_FTCDHMM_API.Shared.Exceptions;

namespace SEP490_FTCDHMM_API.Tests.Services.UserHealthGoalServiceTests
{
    public class SetGoalAsyncTests : UserHealthGoalServiceTestBase
    {
        [Fact]
        public async Task SetGoal_ShouldExpireExistingActiveGoal()
        {
            var userId = NewId();
            var targetId = NewId();

            var activeGoal = new UserHealthGoal
            {
                Id = NewId(),
                UserId = userId,
                ExpiredAtUtc = null,
                Type = HealthGoalType.SYSTEM,
                HealthGoalId = targetId
            };

            UserHealthGoalRepositoryMock
                .Setup(r => r.GetActiveGoalByUserIdAsync(userId))
                .ReturnsAsync(activeGoal);

            UserHealthGoalRepositoryMock
                .Setup(r => r.UpdateAsync(It.Is<UserHealthGoal>(
                    g => g == activeGoal && g.ExpiredAtUtc != null)))
                .Returns(Task.CompletedTask);

            HealthGoalRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<HealthGoal, bool>>>()))
                .ReturnsAsync(true);

            UserHealthGoalRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<UserHealthGoal>()))
                .ReturnsAsync((UserHealthGoal g) => g);

            var req = new UserHealthGoalRequest
            {
                Type = HealthGoalType.SYSTEM.Value,
                ExpiredAtUtc = null
            };

            await Sut.SetGoalAsync(userId, targetId, req);

            UserHealthGoalRepositoryMock.VerifyAll();
            HealthGoalRepositoryMock.VerifyAll();
        }

        [Fact]
        public async Task SetGoal_System_ShouldThrow_WhenSystemGoalNotFound()
        {
            var userId = NewId();
            var targetId = NewId();

            UserHealthGoalRepositoryMock
                .Setup(r => r.GetActiveGoalByUserIdAsync(userId))
                .ReturnsAsync((UserHealthGoal?)null);

            HealthGoalRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<HealthGoal, bool>>>()))
                .ReturnsAsync(false);

            var req = new UserHealthGoalRequest
            {
                Type = HealthGoalType.SYSTEM.Value,
                ExpiredAtUtc = null
            };

            await Assert.ThrowsAsync<AppException>(() => Sut.SetGoalAsync(userId, targetId, req));

            UserHealthGoalRepositoryMock.VerifyAll();
            HealthGoalRepositoryMock.VerifyAll();
        }

        [Fact]
        public async Task SetGoal_System_ShouldSucceed()
        {
            var userId = NewId();
            var targetId = NewId();

            UserHealthGoalRepositoryMock
                .Setup(r => r.GetActiveGoalByUserIdAsync(userId))
                .ReturnsAsync((UserHealthGoal?)null);

            HealthGoalRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<HealthGoal, bool>>>()))
                .ReturnsAsync(true);

            UserHealthGoalRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<UserHealthGoal>()))
                .ReturnsAsync((UserHealthGoal g) => g);

            var req = new UserHealthGoalRequest
            {
                Type = HealthGoalType.SYSTEM.Value,
                ExpiredAtUtc = null
            };

            await Sut.SetGoalAsync(userId, targetId, req);

            UserHealthGoalRepositoryMock.VerifyAll();
            HealthGoalRepositoryMock.VerifyAll();
        }

        [Fact]
        public async Task SetGoal_Custom_ShouldThrow_WhenCustomGoalNotOwned()
        {
            var userId = NewId();
            var targetId = NewId();

            UserHealthGoalRepositoryMock
                .Setup(r => r.GetActiveGoalByUserIdAsync(userId))
                .ReturnsAsync((UserHealthGoal?)null);

            CustomHealthGoalRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<CustomHealthGoal, bool>>>()))
                .ReturnsAsync(false);

            var req = new UserHealthGoalRequest
            {
                Type = HealthGoalType.CUSTOM.Value,
                ExpiredAtUtc = null
            };

            await Assert.ThrowsAsync<AppException>(() => Sut.SetGoalAsync(userId, targetId, req));

            UserHealthGoalRepositoryMock.VerifyAll();
            CustomHealthGoalRepositoryMock.VerifyAll();
        }

        [Fact]
        public async Task SetGoal_Custom_ShouldSucceed()
        {
            var userId = NewId();
            var targetId = NewId();

            UserHealthGoalRepositoryMock
                .Setup(r => r.GetActiveGoalByUserIdAsync(userId))
                .ReturnsAsync((UserHealthGoal?)null);

            CustomHealthGoalRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<CustomHealthGoal, bool>>>()))
                .ReturnsAsync(true);

            UserHealthGoalRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<UserHealthGoal>()))
                .ReturnsAsync((UserHealthGoal g) => g);

            var req = new UserHealthGoalRequest
            {
                Type = HealthGoalType.CUSTOM.Value,
                ExpiredAtUtc = null
            };

            await Sut.SetGoalAsync(userId, targetId, req);

            UserHealthGoalRepositoryMock.VerifyAll();
            CustomHealthGoalRepositoryMock.VerifyAll();
        }

        [Fact]
        public async Task SetGoal_ShouldThrow_WhenExpiredAtInPast()
        {
            var userId = NewId();
            var targetId = NewId();

            var req = new UserHealthGoalRequest
            {
                Type = HealthGoalType.SYSTEM.Value,
                ExpiredAtUtc = DateTime.UtcNow.AddMinutes(-1)
            };

            await Assert.ThrowsAsync<AppException>(() => Sut.SetGoalAsync(userId, targetId, req));
        }
    }
}
