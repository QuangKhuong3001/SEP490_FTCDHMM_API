namespace SEP490_FTCDHMM_API.Application.Configurations
{
    public static class RecommendationBehaviorBuilder
    {
        public static Dictionary<Guid, int> BuildRatingByLabel(
            Dictionary<Guid, int> ratings,
            Dictionary<Guid, List<Guid>> recipeLabelsMap)
        {
            var result = new Dictionary<Guid, int>();

            foreach (var (recipeId, score) in ratings)
            {
                if (!recipeLabelsMap.TryGetValue(recipeId, out var labels))
                    continue;

                foreach (var labelId in labels)
                {
                    if (!result.TryAdd(labelId, score))
                        result[labelId] += score;
                }
            }

            return result;
        }

        public static Dictionary<Guid, int> BuildViewByLabel(
            Dictionary<Guid, int> viewCounts,
            Dictionary<Guid, List<Guid>> recipeLabelsMap)
        {
            var result = new Dictionary<Guid, int>();

            foreach (var (recipeId, count) in viewCounts)
            {
                if (!recipeLabelsMap.TryGetValue(recipeId, out var labels))
                    continue;

                foreach (var labelId in labels)
                {
                    if (!result.TryAdd(labelId, count))
                        result[labelId] += count;
                }
            }

            return result;
        }

        public static Dictionary<Guid, int> BuildCommentByLabel(
            Dictionary<Guid, int> commentCounts,
            Dictionary<Guid, List<Guid>> recipeLabelsMap)
        {
            var result = new Dictionary<Guid, int>();

            foreach (var (recipeId, count) in commentCounts)
            {
                if (!recipeLabelsMap.TryGetValue(recipeId, out var labels))
                    continue;

                foreach (var labelId in labels)
                {
                    if (!result.TryAdd(labelId, count))
                        result[labelId] += count;
                }
            }

            return result;
        }

        public static Dictionary<Guid, int> BuildSaveByLabel(
            HashSet<Guid> savedRecipeIds,
            Dictionary<Guid, List<Guid>> recipeLabelsMap)
        {
            var result = new Dictionary<Guid, int>();

            foreach (var recipeId in savedRecipeIds)
            {
                if (!recipeLabelsMap.TryGetValue(recipeId, out var labels))
                    continue;

                foreach (var labelId in labels)
                {
                    if (!result.TryAdd(labelId, 1))
                        result[labelId] += 1;
                }
            }

            return result;
        }
    }

}
