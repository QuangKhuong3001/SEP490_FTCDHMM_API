using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using SEP490_FTCDHMM_API.Application.Jobs.Interfaces;

namespace SEP490_FTCDHMM_API.Infrastructure.Hangfire
{
    public static class HangfireJobRegistrar
    {
        public static void Register(IServiceProvider provider)
        {
            var job = provider.GetRequiredService<IExpireUserDietRestrictionsJob>();

            RecurringJob.AddOrUpdate(
                recurringJobId: "expire-user-diet-restrictions",
                methodCall: () => job.ExecuteAsync(),
                cronExpression: "6 13 * * *",
                options: new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"),
                    MisfireHandling = MisfireHandlingMode.Strict
                }
            );
        }
    }
}
