namespace SEP490_FTCDHMM_API.Domain.ValueObjects
{
    public record Gender(string Value)
    {
        public static readonly Gender Male = new("MALE");
        public static readonly Gender Female = new("FEMALE");
        public static readonly Gender Other = new("OTHER");

        public override string ToString() => Value;

        public static Gender From(string value)
        {
            return value.Trim().ToUpperInvariant() switch
            {
                "MALE" => Male,
                "FEMALE" => Female,
                _ => Other
            };
        }
    }
}
