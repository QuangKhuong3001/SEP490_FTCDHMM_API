using SEP490_FTCDHMM_API.Domain.Entities;

namespace SEP490_FTCDHMM_API.Application.Interfaces.SystemServices
{
    public interface IRecipeScoringSystem
    {
        double CalculateFinalScore(AppUser user, Recipe recipe);
    }
}
