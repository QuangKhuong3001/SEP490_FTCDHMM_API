using SEP490_FTCDHMM_API.Application.Dtos.HealthGoalDtos;

namespace SEP490_FTCDHMM_API.Application.Interfaces.SystemServices
{
    public interface IHealthGoalCalculator
    {
        HealthGoalCalculationResult Calculate(HealthGoalCalculationRequest request);
    }
}
