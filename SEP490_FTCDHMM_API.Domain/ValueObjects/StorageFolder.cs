namespace SEP490_FTCDHMM_API.Domain.ValueObjects
{
    public record StorageFolder(string Value)
    {
        public static readonly StorageFolder Avatars = new("avatars");

        public override string ToString() => Value;
    }

}
