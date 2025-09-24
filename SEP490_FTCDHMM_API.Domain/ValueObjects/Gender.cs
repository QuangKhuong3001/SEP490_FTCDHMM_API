namespace SEP490_FTCDHMM_API.Domain.ValueObjects
{
    public record Gender(string Value)
    {
        public static readonly Gender Male = new("Male");
        public static readonly Gender Female = new("Female");
        public static readonly Gender Other = new("Other");

        public override string ToString() => Value;

        public static Gender From(string value)
        {
            return value switch
            {
                "Male" => Male,
                "Female" => Female,
                _ => Other
            };
        }
    }
}
