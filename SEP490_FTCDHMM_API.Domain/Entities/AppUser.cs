using Microsoft.AspNetCore.Identity;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class AppUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; } = Gender.Male;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public ActivityLevel ActivityLevel { get; set; } = ActivityLevel.Moderate;
        public string? Address { get; set; }
        public string? Bio { get; set; }
        public string? LockReason { get; set; }
        public Guid? AvatarId { get; set; }
        public Image? Avatar { get; set; } = null!;
        public Guid RoleId { get; set; }
        public AppRole Role { get; set; } = null!;
        public ICollection<UserHealthGoal> UserHealthGoals { get; set; } = new List<UserHealthGoal>();
        public ICollection<UserFollow> Following { get; set; } = new List<UserFollow>();
        public ICollection<UserFollow> Followers { get; set; } = new List<UserFollow>();
        public ICollection<UserRecipeView> ViewedRecipes { get; set; } = new List<UserRecipeView>();
        public ICollection<UserSaveRecipe> SaveRecipes { get; set; } = new List<UserSaveRecipe>();
        public ICollection<UserDietRestriction> DietRestrictions { get; set; } = new List<UserDietRestriction>();
        public ICollection<UserHealthGoal> HealthGoals { get; set; } = new List<UserHealthGoal>();
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
        public ICollection<UserHealthMetric> HealthMetrics { get; set; } = new List<UserHealthMetric>();
    }
}
