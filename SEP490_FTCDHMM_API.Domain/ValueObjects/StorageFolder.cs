﻿namespace SEP490_FTCDHMM_API.Domain.ValueObjects
{
    public record StorageFolder(string Value)
    {
        public static readonly StorageFolder AVATARS = new("avatars");
        public static readonly StorageFolder INGREDIENTS = new("ingredients");
        public static readonly StorageFolder RECIPES = new("recipes");
        public static readonly StorageFolder COOKING_STEPS = new("cookingSteps");

        public override string ToString() => Value;
    }

}
