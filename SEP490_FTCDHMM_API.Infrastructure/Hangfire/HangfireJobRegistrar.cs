using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using SEP490_FTCDHMM_API.Application.Jobs.Interfaces;

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
        }
    }
}
