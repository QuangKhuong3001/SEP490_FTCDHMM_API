using Moq;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Services.Implementations;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Tests.Services.ReportServiceTests
{
    public abstract class ReportServiceTestBase
    {
        protected Mock<IReportRepository> ReportRepoMock { get; }
        protected Mock<IUserRepository> UserRepoMock { get; }
        protected Mock<IRecipeRepository> RecipeRepoMock { get; }
        protected Mock<ICommentRepository> CommentRepoMock { get; }
        protected Mock<IRatingRepository> RatingRepoMock { get; }

        protected ReportService Sut { get; }

        protected ReportServiceTestBase()
        {
            ReportRepoMock = new Mock<IReportRepository>(MockBehavior.Strict);
            UserRepoMock = new Mock<IUserRepository>(MockBehavior.Strict);
            RecipeRepoMock = new Mock<IRecipeRepository>(MockBehavior.Strict);
            CommentRepoMock = new Mock<ICommentRepository>(MockBehavior.Strict);
            RatingRepoMock = new Mock<IRatingRepository>(MockBehavior.Strict);

            Sut = new ReportService(
                ReportRepoMock.Object,
                UserRepoMock.Object,
                RecipeRepoMock.Object,
                CommentRepoMock.Object,
                RatingRepoMock.Object
            );
        }

        protected AppUser CreateUser(Guid id)
        {
            return new AppUser
            {
                Id = id,
                FirstName = "A",
                LastName = "B"
            };
        }

        protected Report CreateReport(Guid reporterId, Guid targetId, string description)
        {
            return new Report
            {
                Id = Guid.NewGuid(),
                ReporterId = reporterId,
                TargetId = targetId,
                TargetType = ReportObjectType.User,
                Description = description,
                Status = ReportStatus.Pending,
                CreatedAtUtc = DateTime.UtcNow
            };
        }
    }
}
