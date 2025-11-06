namespace SEP490_FTCDHMM_API.Domain.ValueObjects
{
    public record DeleteMode(string Value)
    {
        public static readonly DeleteMode Self = new("Self");
        public static readonly DeleteMode RecipeAuthor = new("RecipeAuthor");
        public static readonly DeleteMode Permission = new("Permission");

        public override string ToString() => Value;

        public static DeleteMode From(string value)
        {
            return value switch
            {
                "Self" => Self,
                "RecipeAuthor" => RecipeAuthor,
                "Permission" => Permission,
                _ => throw new ArgumentException($"Invalid delete mode: {value}")
            };
        }
    }
}
