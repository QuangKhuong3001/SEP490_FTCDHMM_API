using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using SEP490_FTCDHMM_API.Application.Jobs.Interfaces;
using SEP490_FTCDHMM_API.Application.Jobs.Interfaces.PreComputedInterfaces;

namespace SEP490_FTCDHMM_API.Infrastructure.Hangfire
{
    public static class HangfireJobRegistrar
    {
        public static void Register(IServiceProvider provider)
        {
            var dietRestrictionsJob = provider.GetRequiredService<IExpireUserDietRestrictionsJob>();

            RecurringJob.AddOrUpdate(
                recurringJobId: "expire-user-diet-restrictions",
                methodCall: () => dietRestrictionsJob.ExecuteAsync(),
                cronExpression: "0 1 * * *",
                options: new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"),
                    MisfireHandling = MisfireHandlingMode.Strict
                }
            );

            var deletedImagesJob = provider.GetRequiredService<IDeletedImagesJob>();

            RecurringJob.AddOrUpdate(
                recurringJobId: "deleted-images",
                methodCall: () => deletedImagesJob.ExecuteAsync(),
                cronExpression: "0 1 * * *",
                options: new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"),
                    MisfireHandling = MisfireHandlingMode.Strict
                }
            );

            var clusterAssignmentJob = provider.GetRequiredService<IClusterAssignmentJob>();

            RecurringJob.AddOrUpdate(
                recurringJobId: "cluster-assignment",
                methodCall: () => clusterAssignmentJob.ExecuteAsync(),
                cronExpression: "40 17 * * *",
                options: new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"),
                    MisfireHandling = MisfireHandlingMode.Strict
                }
            );

            var clusterRecommendationPrecomputeJob =
                provider.GetRequiredService<IClusterRecommendationPrecomputeJob>();

            RecurringJob.AddOrUpdate(
                recurringJobId: "cluster-recommendation-precompute",
                methodCall: () => clusterRecommendationPrecomputeJob.ExecuteAsync(),
                cronExpression: "45 17 * * *",
                options: new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"),
                    MisfireHandling = MisfireHandlingMode.Strict
                }
            );
        }
    }
}
