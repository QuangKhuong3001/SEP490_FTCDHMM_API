using Microsoft.EntityFrameworkCore;
using SEP490_FTCDHMM_API.Application.Interfaces.Persistence;
using SEP490_FTCDHMM_API.Application.Services.Interfaces.RecipeInterface;
using SEP490_FTCDHMM_API.Domain.Entities;
using SEP490_FTCDHMM_API.Shared.Exceptions;

public class RecipeBehaviorService : IRecipeBehaviorService
{
    private readonly IUserRecipeViewRepository _viewRepository;
    private readonly IUserFavoriteRecipeRepository _favoriteRepository;
    private readonly IUserSaveRecipeRepository _saveRepository;
    private readonly IRatingRepository _ratingRepository;
    private readonly IRecipeRepository _recipeRepository;
    private readonly IUserBehaviorRepository _behaviorRepository;

    public RecipeBehaviorService(
        IUserRecipeViewRepository viewRepository,
        IUserFavoriteRecipeRepository favoriteRepository,
        IUserSaveRecipeRepository saveRepository,
        IRatingRepository ratingRepository,
        IRecipeRepository recipeRepository,
        IUserBehaviorRepository behaviorRepository)
    {
        _viewRepository = viewRepository;
        _favoriteRepository = favoriteRepository;
        _saveRepository = saveRepository;
        _ratingRepository = ratingRepository;
        _recipeRepository = recipeRepository;
        _behaviorRepository = behaviorRepository;
    }

    private async Task<UserLabelStat> GetOrCreate(Guid userId, Guid labelId)
    {
        var stat = await _behaviorRepository.GetAsync(userId, labelId);
        if (stat != null)
            return stat;

        stat = new UserLabelStat
        {
            UserId = userId,
            LabelId = labelId
        };

        await _behaviorRepository.AddAsync(stat);
        return stat;
    }

    public async Task RecordViewAsync(Guid userId, Recipe recipe)
    {
        var viewed = await _viewRepository.ExistsAsync(v => v.UserId == userId && v.RecipeId == recipe.Id);

        if (!viewed)
        {
            await _viewRepository.AddAsync(new UserRecipeView
            {
                UserId = userId,
                RecipeId = recipe.Id
            });

            recipe.ViewCount++;
            await _recipeRepository.UpdateAsync(recipe);
        }

        foreach (var label in recipe.Labels.Where(l => !l.IsDeleted))
        {
            var stat = await GetOrCreate(userId, label.Id);
            stat.Views++;
        }

        await _behaviorRepository.SaveChangeAsync();
    }

    public async Task RecordFarvoriteAsync(Guid userId, Guid recipeId)
    {
        var exists = await _favoriteRepository.ExistsAsync(f => f.UserId == userId && f.RecipeId == recipeId);
        if (!exists)
        {
            await _favoriteRepository.AddAsync(new UserFavoriteRecipe
            {
                UserId = userId,
                RecipeId = recipeId
            });
        }

        var recipe = await _recipeRepository.GetByIdAsync(recipeId, include: q => q.Include(r => r.Labels));

        foreach (var label in recipe!.Labels.Where(l => !l.IsDeleted))
        {
            var stat = await GetOrCreate(userId, label.Id);
            stat.Favorites++;
        }

        await _behaviorRepository.SaveChangeAsync();
    }

    public async Task RecordUnFavoriteAsync(Guid userId, Guid recipeId)
    {
        var records = await _favoriteRepository.GetAllAsync(f => f.UserId == userId && f.RecipeId == recipeId);
        if (records.Any())
            await _favoriteRepository.DeleteAsync(records.First());

        var recipe = await _recipeRepository.GetByIdAsync(recipeId, include: q => q.Include(r => r.Labels));

        foreach (var label in recipe!.Labels.Where(l => !l.IsDeleted))
        {
            var stat = await GetOrCreate(userId, label.Id);
            stat.Favorites = Math.Max(0, stat.Favorites - 1);
        }

        await _behaviorRepository.SaveChangeAsync();
    }

    public async Task RecordSaveAsync(Guid userId, Guid recipeId)
    {
        var exists = await _saveRepository.ExistsAsync(s => s.UserId == userId && s.RecipeId == recipeId);
        if (!exists)
        {
            await _saveRepository.AddAsync(new UserSaveRecipe
            {
                UserId = userId,
                RecipeId = recipeId
            });
        }

        var recipe = await _recipeRepository.GetByIdAsync(recipeId, include: q => q.Include(r => r.Labels));

        foreach (var label in recipe!.Labels.Where(l => !l.IsDeleted))
        {
            var stat = await GetOrCreate(userId, label.Id);
            stat.Saves++;
        }

        await _behaviorRepository.SaveChangeAsync();
    }

    public async Task RecordUnsaveAsync(Guid userId, Guid recipeId)
    {
        var records = await _saveRepository.GetAllAsync(s => s.UserId == userId && s.RecipeId == recipeId);
        if (records.Any())
            await _saveRepository.DeleteAsync(records.First());

        var recipe = await _recipeRepository.GetByIdAsync(recipeId, include: q => q.Include(r => r.Labels));

        foreach (var label in recipe!.Labels.Where(l => !l.IsDeleted))
        {
            var stat = await GetOrCreate(userId, label.Id);
            stat.Saves = Math.Max(0, stat.Saves - 1);
        }

        await _behaviorRepository.SaveChangeAsync();
    }

    public async Task RecordRatingAsync(Guid userId, Guid recipeId, int rating)
    {
        var exists = await _ratingRepository.ExistsAsync(r => r.UserId == userId && r.RecipeId == recipeId);
        if (exists)
            throw new AppException(AppResponseCode.INVALID_ACTION, "Bạn đã đánh giá công thức này.");

        await _ratingRepository.AddAsync(new Rating
        {
            UserId = userId,
            RecipeId = recipeId,
            Score = rating
        });

        await RecalculateRecipeRating(recipeId);

        var recipe = await _recipeRepository.GetByIdAsync(recipeId, include: q => q.Include(r => r.Labels));

        foreach (var label in recipe!.Labels.Where(l => !l.IsDeleted))
        {
            var stat = await GetOrCreate(userId, label.Id);
            stat.Ratings++;
            stat.RatingSum += rating;
        }

        await _behaviorRepository.SaveChangeAsync();
    }


    public async Task RecordUpdateRatingAsync(Guid userId, Guid recipeId, int newRating)
    {
        var old = await _ratingRepository.GetLatestAsync(r => r.UserId == userId && r.RecipeId == recipeId);
        if (old == null)
            throw new AppException(AppResponseCode.NOT_FOUND, "Bạn chưa đánh giá công thức này.");

        var oldRating = old.Score;
        old.Score = newRating;

        await _ratingRepository.UpdateAsync(old);
        await RecalculateRecipeRating(recipeId);

        var recipe = await _recipeRepository.GetByIdAsync(recipeId, include: q => q.Include(r => r.Labels));

        foreach (var label in recipe!.Labels.Where(l => !l.IsDeleted))
        {
            var stat = await GetOrCreate(userId, label.Id);
            stat.RatingSum = stat.RatingSum - oldRating + newRating;
        }

        await _behaviorRepository.SaveChangeAsync();
    }


    private async Task RecalculateRecipeRating(Guid recipeId)
    {
        var ratings = await _ratingRepository.GetAllAsync(r => r.RecipeId == recipeId);

        double avg = ratings.Any() ? ratings.Average(r => r.Score) : 0;
        int count = ratings.Count();

        var recipe = await _recipeRepository.GetByIdAsync(recipeId);
        recipe!.AvgRating = avg;
        recipe.RatingCount = count;

        await _recipeRepository.UpdateAsync(recipe);
    }
}
