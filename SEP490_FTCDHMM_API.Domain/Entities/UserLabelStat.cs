namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class UserLabelStat
    {
        public Guid UserId { get; set; }
        public AppUser User { get; set; } = null!;

        public Guid LabelId { get; set; }
        public Label Label { get; set; } = null!;

        public int Views { get; set; }
        public int SearchClicks { get; set; }
        public int Favorites { get; set; }
        public int Saves { get; set; }
        public int Ratings { get; set; }
        public double RatingSum { get; set; }
        public double AvgRating => Ratings == 0 ? 0 : RatingSum / Ratings;
    }
}
