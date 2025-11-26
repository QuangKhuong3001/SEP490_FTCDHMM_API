namespace SEP490_FTCDHMM_API.Application.Dtos.RecipeDtos.Recommentdation
{
    public class UserBehaviorStats
    {
        //public Dictionary<Guid, RecipeBehaviorEntry> Recipes { get; set; } = new();

        public double MaxViews { get; set; }
        public double MaxSearchClicks { get; set; }
        public double MaxLikes { get; set; }
        public double MaxSaves { get; set; }
        public double MaxComments { get; set; }
        public double MaxRatings { get; set; }
    }
}
