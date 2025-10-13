namespace SEP490_FTCDHMM_API.Domain.ValueObjects
{
    public record StorageFolder(string Value)
    {
        public static readonly StorageFolder AVATARS = new("avatars");
        public static readonly StorageFolder INGREDIENTS = new("ingredients");

        public override string ToString() => Value;
    }

}
