//using SEP490_FTCDHMM_API.Application.Interfaces;

//namespace SEP490_FTCDHMM_API.Application.Services.Implementations
//{
//    public class IngredientScoreService : IIngredientScoreService
//    {
//        private readonly IIngredientRepository _ingredientRepo;
//        private readonly IRecipeRepository _recipeRepo;
//        private readonly IUnitOfWork _unitOfWork;

//        public IngredientScoreService(IIngredientRepository ingredientRepo, IRecipeRepository recipeRepo, IUnitOfWork unitOfWork)
//        {
//            _ingredientRepo = ingredientRepo;
//            _recipeRepo = recipeRepo;
//            _unitOfWork = unitOfWork;
//        }

//        public async Task RecomputeAllScoresAsync()
//        {
//            var ingredients = await _ingredientRepo.GetAllAsync();

//            foreach (var i in ingredients)
//            {
//                var usage = await _recipeRepo.CountByIngredientAsync(i.Id);
//                var search = i.SearchCount;
//                var clicks = i.ClickCount;
//                var favorites = i.FavoriteCount;
//                var growth = i.GrowthRate;

//                i.PopularityScore = IngredientScoringService.CalculatePopularityScore(
//                    search, usage, clicks, favorites, 0, 0, growth
//                );

//                await _ingredientRepo.UpdateAsync(i);
//            }

//            await _unitOfWork.SaveChangesAsync();
//        }
//    }
//}
