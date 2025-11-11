using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Application.Dtos.ActivityLevelDtos
{
    public class ActivityLevelResponse
    {
        public ActivityLevel ActivityLevel { get; set; } = ActivityLevel.Moderate;
    }
}
