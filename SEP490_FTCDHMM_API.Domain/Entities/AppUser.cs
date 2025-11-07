using Microsoft.AspNetCore.Identity;
using SEP490_FTCDHMM_API.Domain.ValueObjects;

namespace SEP490_FTCDHMM_API.Domain.Entities
{
    public class AppUser : IdentityUser<Guid>
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; } = Gender.Other;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }
        public ActivityLevel ActivityLevel { get; set; } = ActivityLevel.Moderate;

        public Guid? AvatarId { get; set; }
        public Image? Avatar { get; set; } = null!;

        public Guid RoleId { get; set; }
        public AppRole Role { get; set; } = null!;
        public ICollection<UserFollow> Following { get; set; } = new List<UserFollow>();
        public ICollection<UserFollow> Followers { get; set; } = new List<UserFollow>();
        public ICollection<UserRecipeView> ViewedRecipes { get; set; } = new List<UserRecipeView>();
        public ICollection<UserFavoriteRecipe> FavoriteRecipes { get; set; } = new List<UserFavoriteRecipe>();
        public ICollection<UserSaveRecipe> SaveRecipes { get; set; } = new List<UserSaveRecipe>();


    }
}
