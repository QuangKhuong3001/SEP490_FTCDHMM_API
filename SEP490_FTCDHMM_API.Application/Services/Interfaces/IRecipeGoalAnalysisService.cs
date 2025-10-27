using SEP490_FTCDHMM_API.Application.Dtos.HealthGoalDtos;

namespace SEP490_FTCDHMM_API.Application.Services.Interfaces
{
    public interface IRecipeGoalAnalysisService
    {
        Task<HealthGoalAnalysisResponse> AnalyzeAsync(Guid recipeId, Guid healthGoalId);
    }
}
