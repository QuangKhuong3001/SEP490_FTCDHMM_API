﻿namespace SEP490_FTCDHMM_API.Domain.ValueObjects
{
    public record PermissionValue(string Domain, string Action)
    {
        public override string ToString() => $"{Domain}:{Action}";

        public static readonly PermissionValue Moderator_Create = new("ModeratorManagement", "Create");
        public static readonly PermissionValue Moderator_View = new("ModeratorManagement", "View");
        public static readonly PermissionValue Moderator_Update = new("ModeratorManagement", "Update");
        public static readonly PermissionValue Moderator_Delete = new("ModeratorManagement", "Delete");

        public static readonly PermissionValue Customer_Create = new("CustomerManagement", "Create");
        public static readonly PermissionValue Customer_View = new("CustomerManagement", "View");
        public static readonly PermissionValue Customer_Update = new("CustomerManagement", "Update");
        public static readonly PermissionValue Customer_Delete = new("CustomerManagement", "Delete");

        public static readonly PermissionValue Ingredient_Create = new("Ingredient", "Create");
        public static readonly PermissionValue Ingredient_Update = new("Ingredient", "Update");
        public static readonly PermissionValue Ingredient_Delete = new("Ingredient", "Delete");

        public static readonly PermissionValue Label_Create = new("Label", "Create");
        public static readonly PermissionValue Label_Update = new("Label", "Update");
        public static readonly PermissionValue Label_Delete = new("Label", "Delete");

        public static readonly PermissionValue IngredientCategory_Create = new("IngredientCategory", "Create");
        public static readonly PermissionValue IngredientCategory_Delete = new("IngredientCategory", "Delete");

        public static readonly PermissionValue HealthGoal_Create = new("HealthGoal", "Create");
        public static readonly PermissionValue HealthGoal_Update = new("HealthGoal", "Update");
        public static readonly PermissionValue HealthGoal_Delete = new("HealthGoal", "Delete");

        public static IEnumerable<PermissionValue> All => new[]
        {
            Moderator_Create,
            Moderator_View,
            Moderator_Update,
            Moderator_Delete,
            Customer_Create,
            Customer_Update,
            Ingredient_Create,
            Ingredient_Update,
            Ingredient_Delete,
            Label_Create,
            Label_Delete,
            Label_Update,
            IngredientCategory_Create,
            IngredientCategory_Delete,
            HealthGoal_Create,
            HealthGoal_Delete,
            HealthGoal_Update,
        };
    }
}
