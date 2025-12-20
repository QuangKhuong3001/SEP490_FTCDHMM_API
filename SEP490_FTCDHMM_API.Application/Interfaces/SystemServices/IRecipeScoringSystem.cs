using SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Recommentdation;

namespace SEP490_FTCDHMM_API.Application.Interfaces.SystemServices
{
    public interface IRecipeScoringSystem
    {
        double CalculateFinalScore(RecommendationUserContext user, RecipeScoringSnapshot recipe);
    }
}
